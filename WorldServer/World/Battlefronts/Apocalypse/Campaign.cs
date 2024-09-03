using Common;
using Common.Database.World.Battlefront;
using FrameWork;
using GameData;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SystemData;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Abilities;
using WorldServer.World.Abilities.Buffs;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Battlefronts.Bounty;
using WorldServer.World.Battlefronts.Keeps;
using WorldServer.World.Battlefronts.Objectives;
using WorldServer.World.Interfaces;
using WorldServer.World.Map;
using WorldServer.World.Objects;
using WorldServer.World.Positions;

namespace WorldServer.World.Battlefronts.Apocalypse
{
    public enum ProgressionFlow
    {
        Capital,
        Reset,
        Lock
    }

    /// <summary>
    /// Represents an open RVR front in a given Region (1 Region -> n Zones) Eg Region 14 (T2 Emp -> Zone 101 Troll Country & Zone 107 Ostland
    /// </summary>
    public class Campaign
    {
        public static IObjectDatabase Database = null;

        public static int DOMINATION_POINTS_REQUIRED = Core.Config.DominationPointsRequired;
        public static int FORT_DEFENCE_TIMER = Core.Config.FortDefenceTimer;  // 15 mins is 900k.
        protected static readonly object LockObject = new object();

        protected static readonly Logger BattlefrontLogger = LogManager.GetLogger("BattlefrontLogger");
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public virtual  int PairingId { get; set; }
        public virtual  VictoryPointProgress VictoryPointProgress { get; set; }
        public virtual  RegionMgr Region { get; set; }
        public virtual  IBattleFrontManager BattleFrontManager { get; set; }
        public virtual  IApocCommunications CommunicationsEngine { get; }

        // List of battlefront statuses for this Campaign
        public List<BattleFrontStatus> ApocBattleFrontStatuses => GetBattleFrontStatuses(Region.RegionId);

        /// <summary>
        /// A list of keeps within this Campaign.
        /// </summary>
        public readonly List<BattleFrontKeep> Keeps = new List<BattleFrontKeep>();

        protected readonly EventInterface _EvtInterface = new EventInterface();

        public virtual  SiegeManager SiegeManager { get; set; }

        public HashSet<Player> PlayersInLakeSet;
        public List<BattlefieldObjective> Objectives;

        public ConcurrentDictionary<int, int> OrderPlayerPopulationList = new ConcurrentDictionary<int, int>();
        public ConcurrentDictionary<int, int> DestructionPlayerPopulationList = new ConcurrentDictionary<int, int>();

        protected volatile int _orderCount = 0;
        protected volatile int _destroCount = 0;

        #region Against All Odds

        protected readonly HashSet<Player> _playersInLakeSet = new HashSet<Player>();

        public readonly List<Player> _orderInLake = new List<Player>();
        public readonly List<Player> _destroInLake = new List<Player>();

        public int _totalMaxOrder = 0;
        public int _totalMaxDestro = 0;

        public int AgainstAllOddsMult => AgainstAllOddsTracker.AgainstAllOddsMult;

        #endregion Against All Odds

        public AAOTracker AgainstAllOddsTracker;
        protected RVRRewardManager _rewardManager;

        public virtual  bool DefenderPopTooSmall { get; set; }
        public virtual  int Tier { get; set; }

        public virtual  int DestructionDominationTimerLength { get; set; }
        public virtual  int OrderDominationTimerLength { get; set; }

        public virtual  int DestructionDominationCounter { get; set; }
        public virtual  int OrderDominationCounter { get; set; }

        public virtual  RegionLockManager RegionLockManager { get; set; }
        public virtual  IRewardManager RewardManager { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionMgr"></param>
        /// <param name="objectives"></param>
        /// <param name="players"></param>
        public Campaign(RegionMgr regionMgr,
            List<BattlefieldObjective> objectives,
            HashSet<Player> players,
            IBattleFrontManager bfm,
            IApocCommunications communicationsEngine,
            int pairingId)
        {
            PairingId = pairingId;
            Region = regionMgr;
            VictoryPointProgress = new VictoryPointProgress();
            PlayersInLakeSet = players;
            Objectives = objectives;
            BattleFrontManager = bfm;
            CommunicationsEngine = communicationsEngine;

            Tier = (byte)Region.GetTier();
            PlaceObjectives();

            LoadKeeps();
            AgainstAllOddsTracker = new AAOTracker();
            _rewardManager = new RVRRewardManager();
            SiegeManager = new SiegeManager();

            DestructionDominationCounter = Core.Config.DestructionDominationTimerLength;
            OrderDominationCounter = Core.Config.OrderDominationTimerLength;

            //_EvtInterface.AddEvent(UpdateWanderingMobs, 5000, 0);
            RegionLockManager = new RegionLockManager(Region);
        }

        //public void StartWanderingMobs(int zoneId)
        //{

        //    var activeCreatures = this.Region.GetObjects<Creature>().Where(x => x.ZoneId == zoneId);
        //    foreach (var creature in activeCreatures)
        //    {
        //        creature.SetWander(1);
        //        BattlefrontLogger.Debug($"Setting mob wandering : {creature.Name} {creature.Entry}");
        //    }
        //}

        //public void UpdateWanderingMobs()
        //{
        //    var activeCampaign = BattleFrontManager.GetActiveCampaign();
        //    var status = activeCampaign?.ActiveBattleFrontStatus;
        //    if (status != null)
        //    {
        //        if (activeCampaign != this)
        //            return;

        //        var activeCreatures = this.Region.GetObjects<Creature>().Where(x => x.ZoneId == status.ZoneId && x.IsActive && !x.IsDisposed && !x.IsKeepLord && x.IsCreature());

        //        foreach (var creature in activeCreatures)
        //        {

        //                creature.SetWander(1);
        //                creature.MvtInterface.Move(
        //                    creature.WorldPosition.X + StaticRandom.Instance.Next(2000),
        //                    creature.WorldPosition.Y + StaticRandom.Instance.Next(2000),
        //                    creature.WorldPosition.Z);

        //        }
        //    }
        //}

        protected virtual void RegisterEvents()
        {
            _EvtInterface.AddEvent(UpdateVictoryPoints, 6000, 0);

            _EvtInterface.AddEvent(UpdateBOs, 5000, 0);
            // Tell each player the RVR status
            _EvtInterface.AddEvent(UpdateRVRStatus, 120000, 0);
            // Recalculate AAO
            _EvtInterface.AddEvent(UpdateAAOBuffs, 30000, 0);
            _EvtInterface.AddEvent(DetermineCaptains, 120000, 0);
            // Check RVR zone for highest contributors (Captains)
            _EvtInterface.AddEvent(SavePlayerContribution, 180000, 0);
            // record metrics
            _EvtInterface.AddEvent(RecordMetrics, 600000, 0);
            _EvtInterface.AddEvent(DestructionDominationCheck, 60000, 0);
            _EvtInterface.AddEvent(OrderDominationCheck, 60000, 0);
            _EvtInterface.AddEvent(UpdateCampaignObjectiveBuffs, 10000, 0);
            _EvtInterface.AddEvent(CheckKeepTimers, 10000, 0);
            _EvtInterface.AddEvent(UpdateKeepResources, 60000, 0);
            _EvtInterface.AddEvent(IPCheck, 180000, 0);
            // _EvtInterface.AddEvent(RefreshObjectiveStatus, 20000, 0);
            _EvtInterface.AddEvent(CountdownFortDefenceTimer, FORT_DEFENCE_TIMER, 0);
        }

        /// <summary>
        /// Loop through players in the campaign and if any have the same IP - inform a GM.
        /// </summary>
        protected virtual void IPCheck()
        {
            var hash = new HashSet<string>();
            if (PlayersInLakeSet == null)
                return;
            foreach (var item in PlayersInLakeSet)
            {
                var ipAddress = item?.Client?._Account?.Ip;
                if (ipAddress != "")
                {
                    if (item.Client._Account.GmLevel == 1)
                    {
                        if (!hash.Add(ipAddress))
                        {
                            PlayerUtil.SendGMBroadcastMessage(Player._Players,
                                $"{item.Name} has a duplicate IP address in game.");
                        }
                    }
                }
            }
        }

        protected virtual void DetermineCaptains()
        {
            RealmCaptainManager.DetermineCaptains(BattleFrontManager, Region);
        }

        /// <summary>
        /// If there is a keep under attack, check it's defence timer count down.
        /// </summary>
        protected virtual void CountdownFortDefenceTimer()
        {
            // If its a fort, not locked and the active zone
            foreach (BattleFrontStatus bf in ApocBattleFrontStatuses)
            {
                var k = Keeps.SingleOrDefault(x => x.IsFortress() && (bf.DestroZoneId == x.ZoneId || bf.OrderZoneId == x.ZoneId) && x.KeepStatus != KeepStatus.KEEPSTATUS_LOCKED);
                k?.CountdownFortDefenceTimer((int)FORT_DEFENCE_TIMER / 1000 / 60);
            }
        }

        /// <summary>
        /// Inform all players in the active battlefront about the objective status
        /// </summary>
        protected virtual void RefreshObjectiveStatus()
        {
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                if (status != null)
                {
                    if (status.Locked)
                        continue;

                    lock (status)
                    {
                        BattlefrontLogger.Trace($"Checking RefreshObjectiveStatus...");
                        var playersToAnnounceTo = Player._Players.Where(x => !x.IsDisposed
                                                                             && x.IsInWorld()
                                                                             && x.CbtInterface.IsPvp
                                                                             && x.ScnInterface.Scenario == null
                                                                             && (status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId)
                                                                             && x.Region.RegionId == status.RegionId);

                        foreach (var player in playersToAnnounceTo)
                        {
                            SendObjectives(player, Objectives.Where(x => status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId));
                        }
                    }
                }
            }
        }

        protected virtual void CheckKeepTimers()
        {
            // There is a race condition here.
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;
                        lock (status)
                        {
                            BattlefrontLogger.Trace($"Checking Keep Timers...");
                            if (status.RegionId == Region.RegionId)
                            {
                                foreach (var keep in Keeps)
                                {
                                    if (keep.KeepStatus != KeepStatus.KEEPSTATUS_LOCKED)
                                    {
                                        keep.CheckTimers();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disabled at the moment - intent is to track who was close to the lord for when rewards are handed out.
        /// </summary>
        protected virtual void CheckKeepPlayersInvolved()
        {
            // There is a race condition here.
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        lock (status)
                        {
                            BattlefrontLogger.Trace($"Checking Keep Players Involved...");
                            if (status.RegionId == Region.RegionId)
                            {
                                foreach (var keep in Keeps)
                                {
                                    keep.CheckKeepPlayersInvolved();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual void UpdateKeepResources()
        {
            // There is a race condition here.
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        lock (status)
                        {
                            BattlefrontLogger.Trace($"Checking Keep Resources...");

                            if (NumberDestructionKeepsInZone() == 2)
                            {
                                // Place Siege merchant out the front of the WC
                                // TODO
                            }
                            if (NumberOrderKeepsInZone() == 2)
                            {
                                // Place Siege merchant out the front of the WC
                                // TODO
                            }
                        }
                    }
                }
            }
        }

        public virtual int NumberOrderKeepsInZone()
        {
            var orderCount = 0;

            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        foreach (var keep in status.KeepList)
                        {
                            if ((SetRealms)keep.Realm == SetRealms.REALMS_REALM_ORDER)
                            {
                                orderCount++;
                            }
                        }
                    }
                }
            }
            return orderCount;
        }

        public virtual int NumberDestructionKeepsInZone()
        {
            var destCount = 0;

            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        foreach (var keep in status.KeepList)
                        {
                            if ((SetRealms)keep.Realm == SetRealms.REALMS_REALM_DESTRUCTION)
                            {
                                destCount++;
                            }
                        }
                    }
                }
            }
            return destCount;
        }

        protected virtual void DestructionDominationCheck()
        {
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        // Only worry about the battlefrontstatus in this region.
                        if (status.RegionId != Region.RegionId)
                            continue;

                        if (activeCampaign.Tier == 1)
                            continue;

                        var objectives = activeCampaign.Objectives.Where(x => status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId);
                        foreach (var battlefieldObjective in objectives)
                        {
                            if (battlefieldObjective.State != StateFlags.Locked)
                            {
                                break;
                            }
                            if (battlefieldObjective.OwningRealm != SetRealms.REALMS_REALM_DESTRUCTION)
                            {
                                break;
                            }
                        }
                        var keeps = activeCampaign.Keeps.Where(x => status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId);
                        foreach (var battleFrontKeep in keeps)
                        {
                            if (battleFrontKeep.KeepStatus != KeepStatus.KEEPSTATUS_SAFE)
                            {
                                break;
                            }
                            if (battleFrontKeep.Realm != SetRealms.REALMS_REALM_DESTRUCTION)
                            {
                                break;
                            }

                            if (battleFrontKeep.Fortress)
                                break;
                        }

                        DestructionDominationCounter--;

                        if (DestructionDominationCounter <= 0)
                        {
                            BattlefrontLogger.Info($"Destruction Domination Victory!");
                            NotifyPlayersOfDomination($"Destruction Domination Victory!", status);
                            VictoryPointProgress.DestructionVictoryPoints = BattleFrontConstants.LOCK_VICTORY_POINTS;
                        }
                        else
                        {
                            NotifyPlayersOfDomination($"Destruction is dominating - {DestructionDominationCounter} minutes remain", status);
                        }
                    }
                }
            }
        }

        protected virtual void OrderDominationCheck()
        {
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        // Only worry about the battlefrontstatus in this region.
                        if (status.RegionId != Region.RegionId)
                            continue;

                        if (activeCampaign.Tier == 1)
                            continue;

                        var objectives = activeCampaign.Objectives.Where(x => status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId);
                        foreach (var battlefieldObjective in objectives)
                        {
                            if (battlefieldObjective.State != StateFlags.Locked)
                            {
                                break;
                            }
                            if (battlefieldObjective.OwningRealm != SetRealms.REALMS_REALM_ORDER)
                            {
                                break;
                            }
                        }
                        var keeps = activeCampaign.Keeps.Where(x => status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId);
                        foreach (var battleFrontKeep in keeps)
                        {
                            if (battleFrontKeep.KeepStatus != KeepStatus.KEEPSTATUS_SAFE)
                            {
                                break;
                            }
                            if (battleFrontKeep.Realm != SetRealms.REALMS_REALM_ORDER)
                            {
                                break;
                            }

                            if (battleFrontKeep.Fortress)
                                break;
                        }

                        OrderDominationCounter--;

                        if (OrderDominationCounter <= 0)
                        {
                            BattlefrontLogger.Info($"Order Domination Victory!");
                            NotifyPlayersOfDomination($"Order Domination Victory!", status);
                            VictoryPointProgress.OrderVictoryPoints = BattleFrontConstants.LOCK_VICTORY_POINTS;
                        }
                        else
                        {
                            NotifyPlayersOfDomination($"Order is dominating - {OrderDominationCounter} minutes remain", status);
                        }
                    }
                }
            }
        }

        protected virtual int SecondsToNearestMinute(int seconds)
        {
            return Convert.ToInt32(Math.Round((double)seconds / 60, MidpointRounding.AwayFromZero));
        }

        protected virtual void NotifyPlayersOfDomination(string message, BattleFrontStatus status)
        {
            var playersToNotify = Player._Players.Where(x => !x.IsDisposed
                                                             && x.IsInWorld()
                                                             && x.CbtInterface.IsPvp
                                                             && x.ScnInterface.Scenario == null
                                                             && x.Region.RegionId == status.RegionId
                                                             && status.DestroZoneId == x.ZoneId || status.OrderZoneId == x.ZoneId);

            foreach (var player in playersToNotify)
            {
                player.SendClientMessage(message, ChatLogFilters.CHATLOGFILTERS_RVR);
                BattlefrontLogger.Debug($"{message}");
            }
        }

        protected virtual void BuffAssigned(NewBuff buff)
        {
            var newBuff = buff;
        }

        public virtual void UpdateCampaignObjectiveBuffs()
        {
            // There is a race condition here.
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        lock (status)
                        {
                            BattlefrontLogger.Trace($"Updating Campaign Objective Buffs...");
                            if (status.RegionId == Region.RegionId)
                            {
                                foreach (var objective in Objectives)
                                {
                                    if (objective.BuffId != 0)
                                    {
                                        if (status.OrderZoneId == objective.ZoneId || status.DestroZoneId == objective.ZoneId)
                                        {
                                            if (objective.OwningRealm != SetRealms.REALMS_REALM_NEUTRAL)
                                            {
                                                var buffId = objective.BuffId;
                                                BattlefrontLogger.Trace($"Applying BuffId {buffId} to players.");
                                                var playersToApply = Player._Players.Where(x => !x.IsDisposed
                                                                                                && x.IsInWorld()
                                                                                                && x.CbtInterface.IsPvp
                                                                                                && x.ScnInterface.Scenario == null
                                                                                                && x.Region.RegionId == status.RegionId
                                                                                                && status.OrderZoneId == x.ZoneId || status.DestroZoneId == x.ZoneId
                                                                                                && x.Realm == objective.OwningRealm);

                                                foreach (var player in playersToApply)
                                                {
                                                    player.BuffInterface.QueueBuff(
                                                        new BuffQueueInfo(
                                                            player, player.Level, AbilityMgr.GetBuffInfo((ushort)buffId), BuffAssigned));
                                                }

                                                BattlefrontLogger.Trace($"Removing BuffId {buffId} from opposing players.");

                                                var playersToRemove = Player._Players.Where(x => !x.IsDisposed
                                                                                                 && x.IsInWorld()
                                                                                                 && x.CbtInterface.IsPvp
                                                                                                 && x.ScnInterface.Scenario == null
                                                                                                 && x.Region.RegionId == status.RegionId
                                                                                                 && status.OrderZoneId == x.ZoneId || status.DestroZoneId == x.ZoneId
                                                                                                 && x.Realm != objective.OwningRealm);

                                                foreach (var player in playersToRemove)
                                                {
                                                    player.BuffInterface.RemoveBuffByEntry((ushort)buffId);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void UpdateDoorMsg()
        {
            var oVp = Region.Campaign.VictoryPointProgress.OrderVictoryPoints;
            var dVp = Region.Campaign.VictoryPointProgress.DestructionVictoryPoints;

            //get order/destro keeps
            var oKeep = Region.Campaign.Keeps.FirstOrDefault(x => x.Realm == SetRealms.REALMS_REALM_ORDER);
            var dKeep = Region.Campaign.Keeps.FirstOrDefault(x => x.Realm == SetRealms.REALMS_REALM_DESTRUCTION);

            if (oKeep != null)
            {
                //update keep door health
                foreach (var door in oKeep.Doors)
                {
                    if (!door.GameObject.IsDead)
                    {
                        BattlefrontLogger.Debug("ORDER " + Region.RegionName + " | Door " + door.Info.Number + " Health: " + door.GameObject.Health);
                    }
                }
            }

            if (dKeep != null)
            {
                //update keep door health
                foreach (var door in dKeep.Doors)
                {
                    if (!door.GameObject.IsDead)
                    {
                        BattlefrontLogger.Debug("DESTRO" + Region.RegionName + " | Door " + door.Info.Number + " Health: " + door.GameObject.Health);
                    }
                }
            }
        }

        public virtual void InitializePopulationList(int battlefrontId)
        {
            if (OrderPlayerPopulationList.ContainsKey(battlefrontId))
            {
                OrderPlayerPopulationList[battlefrontId] = 0;
            }
            else
            {
                OrderPlayerPopulationList.TryAdd(battlefrontId, 0);
            }
            if (DestructionPlayerPopulationList.ContainsKey(battlefrontId))
            {
                DestructionPlayerPopulationList[battlefrontId] = 0;
            }
            else
            {
                DestructionPlayerPopulationList.TryAdd(battlefrontId, 0);
            }
        }

        public virtual BattleFrontStatus GetActiveBattleFrontStatus(ushort zoneId)
        {
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                if (status.DestroZoneId == zoneId || status.OrderZoneId == zoneId)
                    return status;
            }
            return null;
        }

        public virtual BattleFrontStatus GetActiveBattleFrontStatus(int pairingId)
        {
            if (ApocBattleFrontStatuses.Count == 0)
            {
                BattlefrontLogger.Error("No BattlefrontStatuses have been created!");
                throw new Exception("No BattlefrontStatuses have been created!");
            }
            try
            {
                return ApocBattleFrontStatuses.Single(x => x.Locked == false && pairingId == x.PairingId);
            }
            catch (Exception e)
            {
                BattlefrontLogger.Warn($"Exception ALL BF Statuses are LOCKED : {e.Message} {e.StackTrace}");
                throw;
            }
        ;
        }

        protected virtual void RecordMetrics()
        {
            CampaignMetrics.RecordMetrics(BattlefrontLogger, Tier, Region, BattleFrontManager);
        }

        /// <summary>
        /// Return the list of Battlefront statuses for a give region.
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public virtual List<BattleFrontStatus> GetBattleFrontStatuses(int regionId)
        {
            return BattleFrontManager.GetBattleFrontStatusList().Where(x => x.RegionId == regionId).ToList();
        }

        protected virtual void PlaceObjectives()
        {
            foreach (var battleFrontObjective in Objectives)
            {
                Region.AddObject(battleFrontObjective, battleFrontObjective.ZoneId);
                battleFrontObjective.BattleFront = this;
            }
        }

        public virtual string GetBattleFrontStatus()
        {
            return $"Victory Points Progress : {VictoryPointProgress.ToString()}";
        }

        protected virtual void UpdateAAOBuffs()
        {
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;

                        List<Player> orderPlayersInZone = new List<Player>();
                        List<Player> destPlayersInZone = new List<Player>();


                        orderPlayersInZone.AddRange(PlayerUtil.GetOrderPlayersInZone(status.OrderZoneId));
                        destPlayersInZone.AddRange(PlayerUtil.GetDestPlayersInZone(status.DestroZoneId));

                        var allPlayersInZone = new List<Player>();
                        allPlayersInZone.AddRange(destPlayersInZone);
                        allPlayersInZone.AddRange(orderPlayersInZone);

                        if (Tier != 1)
                        {
                            BattlefrontLogger.Trace(
                                $"Calculating AAO. Order players : {orderPlayersInZone.Count} Dest players : {destPlayersInZone.Count}");
                        }

                        AgainstAllOddsTracker.RecalculateAAO(allPlayersInZone, orderPlayersInZone.Count, destPlayersInZone.Count);

                        // Used to set keep defence sizes
                        foreach (var keep in Keeps)
                        {
                            keep.UpdateCurrentAAO(AgainstAllOddsTracker.AgainstAllOddsMult);
                        }
                    }
                }
            }
        }

        protected virtual  void SavePlayerContribution()
        {
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);
                foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
                {
                    if (status != null)
                    {
                        if (status.Locked)
                            continue;
                        lock (status.ContributionManagerInstance)
                        {
                            if (status.RegionId == Region.RegionId)
                            {
                                PlayerContributionManager.SavePlayerContribution(status.BattleFrontId, status.ContributionManagerInstance);
                            }
                        }
                    }
                }
            }
        }

        protected virtual  void UpdateRVRStatus()
        {
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                var activeCampaign = BattleFrontManager.GetActiveCampaign(pairing.PairingId);

                // Update players with status of campaign
                foreach (Player plr in Region.Players)
                {
                    if (Region.GetTier() == 1)
                    {
                        plr.SendClientMessage($"RvR Status : {activeCampaign.GetBattleFrontStatus()}", ChatLogFilters.CHATLOGFILTERS_RVR);
                    }
                    else
                    {
                        plr.SendClientMessage($"RvR Status : {activeCampaign.GetBattleFrontStatus()}", ChatLogFilters.CHATLOGFILTERS_RVR);
                    }
                }
            }

            VictoryPointProgress.UpdateStatus(this);
            WorldMgr.UpdateRegionCaptureStatus(WorldMgr.LowerTierCampaignManager, WorldMgr.ScalingCampaignManager);

            // also save into db
            RVRProgressionService.SaveRVRProgression(WorldMgr.LowerTierCampaignManager.BattleFrontProgressions);
            RVRProgressionService.SaveRVRProgression(WorldMgr.ScalingCampaignManager.BattleFrontProgressions);
        }

        /// <summary>
        /// Loads keeps, keep units and doors.
        /// </summary>
        protected virtual  void LoadKeeps()
        {
            List<keep_infos> keeps = BattleFrontService.GetKeepInfos(Region.RegionId);

            if (keeps == null)
                return; // t1 or database lack

            BattlefrontLogger.Debug($"Loading {keeps.Count} keeps for Region {Region.RegionId}");
            foreach (keep_infos info in keeps)
            {
                BattleFrontKeep keep = new BattleFrontKeep(info, (byte)Tier, Region, new KeepCommunications(), info.IsFortress);
                keep.Realm = (SetRealms)keep.Info.Realm;
                keep.PairingId = PairingId;
                Keeps.Add(keep);
                keep.Load();

                Region.AddObject(keep, info.ZoneId);

                if (info.Creatures != null)
                {
                    BattlefrontLogger.Trace($"Adding {info.Creatures.Count} mobs for Keep {info.KeepId}");
                    foreach (keep_creatures crea in info.Creatures)
                    {
                        if (!crea.IsPatrol)
                        {
                            keep.Creatures.Add(new KeepNpcCreature(Region, crea, keep));
                        }
                    }
                }

                if (info.Doors != null)
                {
                    BattlefrontLogger.Trace($"Adding {info.Doors.Count} doors for Keep {info.KeepId}");
                    foreach (keep_doors door in info.Doors)
                    {
                        keep.Doors.Add(new KeepDoor(Region, door, keep));
                    }
                }
            }
        }

        public virtual  BattleFrontKeep GetClosestKeep(Point3D destPos, ushort zoneId, KeepStatus excludedKeepStatus = KeepStatus.KEEPSTATUS_RUINED)
        {
            BattleFrontKeep bestKeep = null;
            ulong bestDist = 0;

            foreach (var keep in Keeps)
            {
                ulong curDist = keep.GetDistanceSquare(destPos);

                if (bestKeep == null || curDist < bestDist)
                {
                    // Dont process keeps that are in excluded status
                    if (keep.KeepStatus == excludedKeepStatus)
                        continue;

                    if (keep.ZoneId == zoneId)
                    {
                        bestKeep = keep;
                        bestDist = keep.GetDistanceSquare(destPos);
                    }
                }
            }

            return bestKeep;
        }

        public virtual  BattleFrontKeep GetClosestFriendlyKeep(Point3D destPos, SetRealms myRealm)
        {
            BattleFrontKeep bestKeep = null;
            ulong bestDist = 0;

            foreach (var keep in Keeps)
            {
                if (keep.Realm == myRealm)
                {
                    ulong curDist = keep.GetDistanceSquare(destPos);

                    if (bestKeep == null || curDist < bestDist)
                    {
                        bestKeep = keep;
                        bestDist = keep.GetDistanceSquare(destPos);
                    }
                }
            }

            return bestKeep;
        }

        public virtual  List<BattleFrontKeep> GetZoneKeeps(ushort zoneId)
        {
            return Keeps.Where(x => x.ZoneId == zoneId).ToList();
        }

        //public virtual  void WriteCaptureStatus(PacketOut Out)
        //{
        //    // Not implemented.
        //    BattlefrontLogger.Trace(".");
        //}

        /// <summary>
        /// Writes the current zone capture status (gauge in upper right corner of client UI).
        /// </summary>
        /// <param name="Out">Packet to write</param>
        /// <param name="lockingRealm">Realm that is locking the Campaign</param>
        public virtual  void WriteCaptureStatus(PacketOut Out, SetRealms lockingRealm)
        {
            BattlefrontLogger.Trace(".");
            Out.WriteByte(0);
            float orderPercent, destroPercent = 0.0f;
            switch (lockingRealm)
            {
                case SetRealms.REALMS_REALM_ORDER:
                    orderPercent = 80;
                    destroPercent = 20;
                    break;

                case SetRealms.REALMS_REALM_DESTRUCTION:
                    orderPercent = 20;
                    destroPercent = 80;
                    break;

                default:
                    orderPercent = (VictoryPointProgress.OrderVictoryPoints * 100) / BattleFrontConstants.LOCK_VICTORY_POINTS;
                    destroPercent = (VictoryPointProgress.DestructionVictoryPoints * 100) / BattleFrontConstants.LOCK_VICTORY_POINTS;
                    break;
            }

            //BattlefrontLogger.Debug($"{ActiveZoneName} : {(byte)orderPercent} {(byte)destroPercent}");

            Out.WriteByte((byte)orderPercent);
            Out.WriteByte((byte)destroPercent);
        }

        public virtual  void Update(long tick)
        {
            _EvtInterface.Update(tick);
            RegionLockManager.Update(tick);
        }

        /// <summary>
        /// Notifies the given player has entered the lake,
        /// removing it from the Campaign's active players list and setting the rvr buff(s).
        /// </summary>
        /// <param name="plr">Player to add, not null</param>
        public virtual  void NotifyEnteredLake(Player plr)
        {
            if (plr == null)
                return;

            if (!plr.ValidInTier(Tier, true))
                return;

            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                if (status != null && !status.Locked)
                {
                    // Player list tracking
                    lock (PlayersInLakeSet)
                    {
                        if (PlayersInLakeSet.Add(plr))
                        {

                            // Which battlefrontId?
                            var battleFrontId = status.BattleFrontId;
                            BattlefrontLogger.Info($"{plr.Name} {plr.Realm} BF Id : {battleFrontId}");
                            try
                            {
                                if (plr.Realm == SetRealms.REALMS_REALM_ORDER)
                                {
                                    OrderPlayerPopulationList[battleFrontId] += 1;
                                    _orderCount++;
                                }
                                else
                                {
                                    DestructionPlayerPopulationList[battleFrontId] += 1;
                                    _destroCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                BattlefrontLogger.Debug($"{OrderPlayerPopulationList.Count} {DestructionPlayerPopulationList.Count}");
                                BattlefrontLogger.Debug($"Could not add {plr.Name} to PopulationList. ");
                                BattlefrontLogger.Warn($"{ex.Message}");
                            }

                            // Tell the player about the objectives.
                            SendObjectives(plr);
                            // Update worldmap
                            WorldMgr.UpdateRegionCaptureStatus(WorldMgr.LowerTierCampaignManager, WorldMgr.ScalingCampaignManager);
                        }
                    }


                    // Buffs
                    plr.BuffInterface.QueueBuff(new BuffQueueInfo(plr, plr.Level, AbilityMgr.GetBuffInfo((ushort)GameBuffs.FieldOfGlory), FogAssigned));

                    // AAO
                    AgainstAllOddsTracker.NotifyEnteredLake(plr);
                }
            }
        }

        /// <summary>
        /// Invoked by buff interface to remove field of glory if necessary.
        /// </summary>
        /// <param name="fogBuff">Buff that was created</param>
        public virtual  void FogAssigned(NewBuff fogBuff)
        {
            if (fogBuff == null || !(fogBuff.Caster is Player))
                return;

            lock (PlayersInLakeSet)
            {
                if (!PlayersInLakeSet.Contains(fogBuff.Caster))
                    fogBuff.BuffHasExpired = true;
            }
        }

        /// <summary>
        /// Notifies the given player has left the lake,
        /// removing it from the Campaign's active players lift and removing the rvr buff(s).
        /// </summary>
        /// <param name="plr">Player to remove, not null</param>
        public virtual  void NotifyLeftLake(Player plr)
        {
            if (!plr.ValidInTier(Tier, true))
                return;
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                if (status != null && !status.Locked)
                {
                    // Player list tracking
                    lock (PlayersInLakeSet)
                    {
                        if (PlayersInLakeSet.Remove(plr))
                        {
                            // Which battlefrontId?
                            var battleFrontId = status.BattleFrontId;

                            try
                            {
                                if (plr.Realm == SetRealms.REALMS_REALM_ORDER)
                                {
                                    OrderPlayerPopulationList[battleFrontId] -= 1;
                                    _orderCount--;
                                }
                                else
                                {
                                    DestructionPlayerPopulationList[battleFrontId] -= 1;
                                    _destroCount--;
                                }
                            }
                            catch { }
                        }
                    }

                    // Buffs
                    plr.BuffInterface.RemoveBuffByEntry((ushort)GameBuffs.FieldOfGlory);

                    // AAO
                    AgainstAllOddsTracker.NotifyLeftLake(plr);
                }
            }
        }

        public virtual  void LockBattleObjectivesByZone(int zoneId, SetRealms realm)
        {
            foreach (var flag in Objectives)
            {
                if ((flag.ZoneId != zoneId) && (flag.RegionId == Region.RegionId))
                {
                    flag.OwningRealm = realm;
                    flag.SetObjectiveLocked();
                }
            }
        }

        public virtual  void LockBattleObjective(SetRealms realm, int objectiveToLock)
        {
            BattlefrontLogger.Debug($"Locking Battle Objective : {realm.ToString()}...");

            foreach (var flag in Objectives)
            {
                if (flag.Id == objectiveToLock)
                {
                    flag.OwningRealm = realm;
                    flag.SetObjectiveLocked();
                }
            }
        }

        /// <summary>
        /// Lock, Advance and handle rewards for Lock of Battlefront
        /// </summary>
        /// <param name="lockingRealm"></param>
        public virtual  void LockBattleFront(rvr_progression activeBattlefront, SetRealms lockingRealm, int forceNumberBags = 0)
        {
            var lockId = WriteZoneLockSummary(activeBattlefront, lockingRealm);

            BattlefrontLogger.Info($"*************************BATTLEFRONT LOCK-START [LockId:{lockId}]*******************");
            BattlefrontLogger.Info($"forceNumberBags = {forceNumberBags}");
            BattlefrontLogger.Info($"Locking Battlefront {activeBattlefront.Description} to {lockingRealm.ToString()}...");

            string message = string.Concat(activeBattlefront.Description, " locked by ", (lockingRealm == SetRealms.REALMS_REALM_ORDER ? "Order" : "Destruction"), "!");

            BattlefrontLogger.Debug(message);

            if (PlayersInLakeSet == null)
                BattlefrontLogger.Warn($"No players in the Lake!!");
            if (_rewardManager == null)
                BattlefrontLogger.Warn($"_rewardManager is null!!");

            BattlefrontLogger.Info($"*************************BATTLEFRONT LOCK-END [LockId:{lockId}] *********************");
        }

        protected virtual  long WriteZoneLockSummary(rvr_progression activeBattlefront, SetRealms lockingRealm)
        {
            var lockId = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"));
            BattleFrontStatus status = BattleFrontManager.GetBattleFrontStatus(activeBattlefront.BattleFrontId);
            try
            {
                var zonelockSummary = new rvr_zone_lock_summary
                {
                    LockId = lockId,
                    Description = $"Locking Battlefront {activeBattlefront.Description} to {lockingRealm.ToString()}...",
                    DestroVP = (int)status.DestructionVictoryPointPercentage,
                    OrderVP = (int)status.OrderVictoryPointPercentage,
                    LockingRealm = (int)lockingRealm,
                    RegionId = activeBattlefront.RegionId,
                    Timestamp = DateTime.Now,
                    TotalPlayersAtLock = 0,//PlayerUtil.GetAllFlaggedPlayersInZone(BattleFrontManager.ActiveBattleFront.ZoneId).Count(),
                    EligiblePlayersAtLock = status.ContributionManagerInstance.GetEligiblePlayers(0).Count()
                };

                WorldMgr.Database.AddObject(zonelockSummary);
                BattlefrontLogger.Info($"Writing ZoneLockSummary. Lockid = {lockId}...");

                return lockId;
            }
            catch (Exception ex)
            {
                BattlefrontLogger.Error($"Could not write ZoneLockSummary {ex.Message} {ex.StackTrace}");
                return lockId;
            }
        }

        /// <summary>
        /// Generate zone lock rewards.
        /// </summary>
        /// <param name="lockingRealm"></param>
        /// <param name="zoneId"></param>
        /// <param name="orderLootChest"></param>
        /// <param name="destructionLootChest"></param>
        /// <param name="lootOptions"></param>
        /// <param name="forceNumberBags">By default 0 allows the system to decide the number of bags, setting to -1 forces no rewards.</param>
        protected virtual  void GenerateZoneLockRewards(SetRealms lockingRealm, int zoneId, int pairingId)
        {
            try
            {
                var eligiblitySplits =
                    Region.Campaign.GetActiveBattleFrontStatus(pairingId).ContributionManagerInstance.DetermineEligiblePlayers(BattlefrontLogger, lockingRealm);

                // Distribute RR, INF, etc to contributing players
                // Get All players in the zone and if they are not in the eligible list, they receive minor awards
                var allPlayersInZone = PlayerUtil.GetAllFlaggedPlayersInZone((ushort)zoneId);

                Region.Campaign.GetActiveBattleFrontStatus(pairingId).RewardManagerInstance.DistributeZoneFlipBaseRewards(
                    eligiblitySplits.Item3,
                    eligiblitySplits.Item2,
                    lockingRealm,
                    Region.Campaign.GetActiveBattleFrontStatus(pairingId).ContributionManagerInstance.GetMaximumContribution(),
                    Tier == 1 ? 0.5f : 1f,
                    allPlayersInZone);

                var fortZones = new List<int> { 4, 10, 104, 110, 204, 210 };
                if (fortZones.Contains((ushort)zoneId))
                {
                    return;
                }

                // For all eligible players present them with 5 invader crests (only for non-fort zones)
                foreach (var player in eligiblitySplits.Item1)
                {
                    try
                    {
                        var zoneDescription = Region.Campaign.GetActiveBattleFrontStatus(pairingId)?.Description;
                        Logger.Debug($"Assigning Invader Crests for Zone Flip {player.Key.Name}");
                        player.Key.SendClientMessage($"You have been awarded 5 Invader Crests - check your mail.", ChatLogFilters.CHATLOGFILTERS_LOOT);
                        Region.Campaign.GetActiveBattleFrontStatus(pairingId).RewardManagerInstance.MailItem(player.Key.CharacterId, ItemService.GetItem_Info(208453), 5, zoneDescription, "Zone Flip", "Invader crests");

                        RecordZoneLockEligibilityHistory(player, lockingRealm, Region.Campaign.GetActiveBattleFrontStatus(pairingId).DestroZoneId);
                        RecordZoneLockEligibilityHistory(player, lockingRealm, Region.Campaign.GetActiveBattleFrontStatus(pairingId).OrderZoneId);

                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Could not mail invader crests to {player.Key.CharacterId} {ex.Message} {ex.StackTrace}");
                    }
                }
            }
            catch (Exception e)
            {
                BattlefrontLogger.Error($" GenerateZoneLockRewards : {e.Message} {e.StackTrace}");
                throw;
            }
        }

        protected virtual  void RecordZoneLockEligibilityHistory(KeyValuePair<Player, int> player, SetRealms lockingRealm, int zoneId)
        {
            var zone = ZoneService.GetZone_Info((ushort)zoneId);

            var zoneLockEligibility = new rvr_zone_lock_eligibility_history
            {
                CharacterId = (int)player.Key.CharacterId,
                CharacterName = player.Key.Name,
                ContributionValue = player.Value,
                LockingRealm = (int)lockingRealm,
                Timestamp = DateTime.UtcNow,
                ZoneId = zoneId,
                ZoneName = zone.Name,
                Dirty = true
            };
            WorldMgr.Database.AddObject(zoneLockEligibility);
        }

        public virtual  void ClearDictionaries(int pairingId)
        {
            BattleFrontStatus status = Region.Campaign.GetActiveBattleFrontStatus(pairingId);
            status.ContributionManagerInstance.ContributionDictionary.Clear();
            status.DestructionRealmCaptain = null;
            status.OrderRealmCaptain = null;
            BattleFrontManager.BountyManagerInstance.BountyDictionary.Clear();
            SiegeManager.DestroyAllSiege();
            SiegeManager = new SiegeManager();  //HACK TODO : fix
                                                // Remove rvr player contribution.
            SavePlayerContribution();

            BattlefrontLogger.Debug($"RVR Player Contribution, Contribution and Bounty Dictionaries cleared");
        }

        /// <summary>
        /// Helper function to determine whether the active battlefront progression associated with this battlefront is locked.
        /// </summary>
        /// <returns></returns>
        public virtual  bool IsBattleFrontLocked(ushort zoneId)
        {
            foreach (BattleFrontStatus status in BattleFrontManager.GetBattleFrontStatusList())
            {
                if (status.DestroZoneId == zoneId || status.OrderZoneId == zoneId)
                {
                    return status.Locked;
                }
            }
            return true;
        }

        /// <summary>
        /// Helper function to determine whether the active battlefront progression associated with this battlefront is locked.
        /// </summary>
        /// <returns></returns>
        public virtual  bool IsBattleFrontLocked(int id)
        {
            return BattleFrontManager.IsBattleFrontLocked(id);
        }

        /// <summary>
        /// A scale factor for the general reward received from capturing a Battlefield Objective, which increases as more players join the zone.
        /// </summary>
        public virtual  float PopulationScaleFactor { get; private set; }

        /// <summary>
        /// A scale factor determined by the population ratio between the SetRealms as determined by the maximum players they fielded over the last 15 minutes.
        /// </summary>
        public int _relativePopulationFactor;

        /// <summary>
        /// Returns the enemy lockingRealm's population divided by the input lockingRealm's population.
        /// </summary>
        protected virtual  float GetRelativePopFactor(SetRealms realm)
        {
            if (_relativePopulationFactor == 0)
                return 0;
            return realm == SetRealms.REALMS_REALM_DESTRUCTION ? _relativePopulationFactor : 1f / _relativePopulationFactor;
        }

        /// <summary>
        /// Sends information to a player about the objectives within a Campaign upon their entry.
        /// </summary>
        /// <param name="plr"></param>
        public virtual  void SendObjectives(Player plr, IEnumerable<BattlefieldObjective> filteredObjectives = null)
        {
            if (filteredObjectives == null)
            {
                foreach (var bo in Objectives)
                    bo.SendState(plr, false);
            }
            else
            {
                foreach (var bo in filteredObjectives)
                {
                    bo.SendState(plr, false);
                }
            }
        }

        protected virtual void UpdateBOs()
        {
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                // RegionLockManager by Order/Dest
                if (IsBattleFrontLocked(status.BattleFrontId))
                    continue; // Nothing to do

                // Only update an active battlefront
                if (status.RegionId != Region.RegionId)
                    continue;

                foreach (var flag in Objectives)
                {
                    if (flag.battleFrontStatus == status)
                    {
                        flag.Update(TCPManager.GetTimeStampMS());
                    }
                }
            }
        }

        /// <summary>
        ///  Updates the victory points per lockingRealm and fires lock when necessary.
        /// </summary>
        protected virtual  void UpdateVictoryPoints()
        {
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                BattlefrontLogger.Trace($"Updating Victory Points for {status.Description}");
                // RegionLockManager by Order/Dest
                if (IsBattleFrontLocked(status.BattleFrontId))
                    continue; // Nothing to do

                // Only update an active battlefront
                if (status.RegionId != Region.RegionId)
                    continue;

                // Victory depends on objective securization in t1
                float orderVictoryPoints = VictoryPointProgress.OrderVictoryPoints;
                float destroVictoryPoints = VictoryPointProgress.DestructionVictoryPoints;

                // Victory points update
                VictoryPointProgress.OrderVictoryPoints = Math.Min(BattleFrontConstants.LOCK_VICTORY_POINTS, orderVictoryPoints);
                VictoryPointProgress.DestructionVictoryPoints = Math.Min(BattleFrontConstants.LOCK_VICTORY_POINTS, destroVictoryPoints);

                // update also rvr progression
                status.Progression.OrderVP = (int)Math.Round(VictoryPointProgress.OrderVictoryPoints);
                status.Progression.DestroVP = (int)Math.Round(VictoryPointProgress.DestructionVictoryPoints);

                ///
                /// Check to Lock and Advance the Battlefront
                ///
                if (VictoryPointProgress.OrderVictoryPoints >= BattleFrontConstants.LOCK_VICTORY_POINTS)
                {
                    Point3D orderWarcampEntrance = null;
                    Point3D destructionWarcampEntrance = null;

                    orderWarcampEntrance = BattleFrontService.GetWarcampEntrance(
                                       (ushort)status.OrderZoneId, SetRealms.REALMS_REALM_ORDER);
                    destructionWarcampEntrance = BattleFrontService.GetWarcampEntrance(
                                       (ushort)status.DestroZoneId, SetRealms.REALMS_REALM_DESTRUCTION);

                    if (orderWarcampEntrance == null)
                    {
                        BattlefrontLogger.Error($"orderWarcampEntrance is null. BattleFrontId: {(ushort)status.BattleFrontId} ");
                    }

                    if (destructionWarcampEntrance == null)
                    {
                        BattlefrontLogger.Error($"destructionWarcampEntrance is null. BattleFrontId: {(ushort)status.BattleFrontId} ");
                    }

                    try
                    {
                        // TODO : This is a bit of a hack - assumes that if the WC entrances are null, this is a fort.
                        if ((orderWarcampEntrance == null) && (destructionWarcampEntrance == null))
                        {
                            ExecuteBattleFrontLock(status, SetRealms.REALMS_REALM_ORDER, null, null, RVRZoneRewardService.RVRRewardFortItems);
                        }
                        else
                        {
                            var tuple = PlaceChestsAtWarcampEntrances(status, orderWarcampEntrance, destructionWarcampEntrance);
                            ExecuteBattleFrontLock(status, SetRealms.REALMS_REALM_ORDER, tuple.Item1, tuple.Item2,
                                RVRZoneRewardService.RVRRewardKeepItems);
                        }
                    }
                    catch (Exception e)
                    {
                        BattlefrontLogger.Error($"Attempt to lock and advance BF failed. {e.Message} {e.StackTrace}");
                        throw;
                    }
                }
                else if (VictoryPointProgress.DestructionVictoryPoints >=
                         BattleFrontConstants.LOCK_VICTORY_POINTS)
                {
                    Point3D orderWarcampEntrance = null;
                    Point3D destructionWarcampEntrance = null;
                    orderWarcampEntrance = BattleFrontService.GetWarcampEntrance(
                                          (ushort)status.OrderZoneId, SetRealms.REALMS_REALM_ORDER);
                    destructionWarcampEntrance = BattleFrontService.GetWarcampEntrance(
                                       (ushort)status.DestroZoneId, SetRealms.REALMS_REALM_DESTRUCTION);
                    if (orderWarcampEntrance == null)
                    {
                        BattlefrontLogger.Error($"orderWarcampEntrance is null. BattleFrontId: {(ushort)status.BattleFrontId} ");
                    }

                    if (destructionWarcampEntrance == null)
                    {
                        BattlefrontLogger.Error($"destructionWarcampEntrance is null. BattleFrontId: {(ushort)status.BattleFrontId} ");
                    }

                    try
                    {
                        // TODO : This is a bit of a hack - assumes that if the WC entrances are null, this is a fort.
                        if ((orderWarcampEntrance == null) && (destructionWarcampEntrance == null))
                        {
                            ExecuteBattleFrontLock(status, SetRealms.REALMS_REALM_DESTRUCTION, null, null, RVRZoneRewardService.RVRRewardFortItems);
                        }
                        else
                        {
                            var tuple = PlaceChestsAtWarcampEntrances(status, orderWarcampEntrance, destructionWarcampEntrance);
                            ExecuteBattleFrontLock(status, SetRealms.REALMS_REALM_DESTRUCTION, tuple.Item1, tuple.Item2, RVRZoneRewardService.RVRRewardKeepItems);
                        }
                    }
                    catch (Exception e)
                    {
                        BattlefrontLogger.Error($"Attempt to lock and advance BF failed. {e.Message} {e.StackTrace}");
                        throw;
                    }
                }
            }
        }

        protected virtual  Tuple<LootChest, LootChest> PlaceChestsAtWarcampEntrances(BattleFrontStatus status, Point3D orderWarcampEntrance, Point3D destructionWarcampEntrance)
        {
            LootChest orderLootChest = null;
            LootChest destructionLootChest = null; ;

            if (orderWarcampEntrance != null)
            {
                orderLootChest = LootChest.Create(
                    Region,
                    orderWarcampEntrance,
                    (ushort)status.OrderZoneId);

                orderLootChest.Title = $"Zone Assault {status.Description}";
                orderLootChest.Content = $"Zone Assault Rewards";
                orderLootChest.SenderName = $"{status.Description}";
            }

            if (destructionWarcampEntrance != null)
            {
                destructionLootChest = LootChest.Create(
                    Region,
                    destructionWarcampEntrance,
                    (ushort)status.DestroZoneId);

                destructionLootChest.Title = $"Zone Assault {status.Description}";
                destructionLootChest.Content = $"Zone Assault Rewards";
                destructionLootChest.SenderName = $"{status.Description}";
            }

            return new Tuple<LootChest, LootChest>(orderLootChest, destructionLootChest);
        }
        public virtual  void ExecuteBattleFrontLock(int pairingId, SetRealms lockingRealm, LootChest orderLootChest, LootChest destructionLootChest, List<RVRRewardItem> lootOptions, int forceNumberBags = 0)
        {
            BattleFrontStatus status = GetActiveBattleFrontStatus(pairingId);
            ExecuteBattleFrontLock(status, lockingRealm, orderLootChest, destructionLootChest, lootOptions);
        }

        public virtual  void ExecuteBattleFrontLock(BattleFrontStatus status, SetRealms lockingRealm, LootChest orderLootChest, LootChest destructionLootChest, List<RVRRewardItem> lootOptions, int forceNumberBags = 0)
        {
            var oldBattleFront = BattleFrontManager.GetActiveBattleFrontFromProgression(status.Progression.PairingId);
            BattlefrontLogger.Info($"Executing BattleFront Lock on {oldBattleFront.Description} for {lockingRealm}");
            Logger.Info($"***Executing BattleFront Lock on {oldBattleFront.Description} for {lockingRealm}***");
            // Must be called before locking the battlefront
            GenerateZoneLockRewards(lockingRealm, oldBattleFront.OrderZoneId, status.PairingId);
            GenerateZoneLockRewards(lockingRealm, oldBattleFront.DestroZoneId, status.PairingId);
            BattleFrontManager.LockActiveBattleFront(status.Progression.PairingId, lockingRealm, forceNumberBags);
            // Remove eligible players.
            ClearDictionaries(status.PairingId);

            // Select the next Progression
            var nextBattleFront = BattleFrontManager.AdvanceBattleFront(status.Progression.PairingId, lockingRealm);

            // If the next rvr_progression is the Reset progression, then check for capital siege
            if (nextBattleFront == null) //We 're reached last battlefront, locking whole pairing
            {
                BattlefrontLogger.Info($"{(Pairing)status.Progression.PairingId} completed rotation! Waiting for capital siege to start...");

                /*

                // Set all regions back to their default owners.
                foreach (var progression in WorldMgr.ScalingCampaignManager.BattleFrontProgressions)
                {
                    if (progression.Tier != 1)
                    {
                        progression.DestroVP = 0;
                        progression.OrderVP = 0;
                        progression.LastOpenedZone = 0;
                        progression.LastOwningRealm = progression.DefaultRealmLock;

                        if (progression.ResetProgressionOnEntry == 1) // PRAAG
                        {
                            progression.LastOpenedZone = 1;
                            WorldMgr.ScalingCampaignManager.ActiveBattleFront = progression;
                            WorldMgr.ScalingCampaignManager.GetActiveCampaign().Keeps.SingleOrDefault(x => x.Info.KeepId == progression.OrderKeepId).Realm = SetRealms.REALMS_REALM_ORDER;
                            WorldMgr.ScalingCampaignManager.GetActiveCampaign().Keeps.SingleOrDefault(x => x.Info.KeepId == progression.OrderKeepId).SetKeepSafe();
                            WorldMgr.ScalingCampaignManager.GetActiveCampaign().Keeps.SingleOrDefault(x => x.Info.KeepId == progression.OrderKeepId).Realm = SetRealms.REALMS_REALM_DESTRUCTION;
                            WorldMgr.ScalingCampaignManager.GetActiveCampaign().Keeps.SingleOrDefault(x => x.Info.KeepId == progression.DestroKeepId).SetKeepSafe();
                            var objectives = WorldMgr.ScalingCampaignManager.GetActiveCampaign().Objectives
                                .Where(x => x.ZoneId == progression.DestroZoneId || x.ZoneId == progression.OrderZoneId);
                            foreach (var battlefieldObjective in objectives)
                            {
                                battlefieldObjective.SetObjectiveSafe();
                            }
                        }

                        var status2 = WorldMgr.ScalingCampaignManager.GetBattleFrontStatusList().SingleOrDefault(x => x.BattleFrontId == progression.BattleFrontId);
                        if (status2 != null)
                        {
                            status2.Locked = true;
                            status2.OpenTimeStamp = FrameWork.TCPManager.GetTimeStamp();
                            status2.LockingRealm = (SetRealms)progression.DefaultRealmLock;
                            status2.FinalVictoryPoint = new VictoryPointProgress();
                            status2.LockTimeStamp = 0;
                            // Reset the population for the battle front status
                            WorldMgr.ScalingCampaignManager.GetActiveCampaign(status2.PairingId).InitializePopulationList(status.BattleFrontId);
                        }
                
                    }
                }*/
            }
            else
            {
                // Set the RVR Progression table values.
                BattleFrontManager.UpdateRVRPRogression(lockingRealm, oldBattleFront, nextBattleFront);
                // Tell the players
                SendCampaignMovementMessage(nextBattleFront);
                // Unlock the next Progression
                BattleFrontManager.OpenActiveBattlefront();

            }
            // This is kind of nasty, should use an event to signal the WorldMgr
            // Tell the server that the RVR status has changed.
            WorldMgr.UpdateRegionCaptureStatus(WorldMgr.LowerTierCampaignManager, WorldMgr.ScalingCampaignManager);
            // Logs the status of all battlefronts known to the Battlefront Manager.
            // BattleFrontManager.AuditBattleFronts(this.Tier);
        }

        protected virtual  void SendCampaignMovementMessage(rvr_progression nextBattleFront)
        {
            var campaignMoveMessage = $"The campaign has moved to {nextBattleFront.Description}";
            BattlefrontLogger.Info(campaignMoveMessage);
            CommunicationsEngine.Broadcast(campaignMoveMessage, Tier);
        }

        public virtual  int GetZoneOwnership(ushort zoneId)
        {
            BattlefrontLogger.Trace($"GetZoneOwnership {zoneId}");
            const int ZONE_STATUS_CONTESTED = 0;
            const int ZONE_STATUS_ORDER_LOCKED = 1;
            const int ZONE_STATUS_DESTRO_LOCKED = 2;

            byte orderKeepsOwned = 0;
            byte destroKeepsOwned = 0;

            if (orderKeepsOwned == 2)
            {
                return ZONE_STATUS_ORDER_LOCKED;
            }
            if (destroKeepsOwned == 2)
            {
                return ZONE_STATUS_DESTRO_LOCKED;
            }
            return ZONE_STATUS_CONTESTED;
        }

        public virtual  void WriteBattleFrontStatus(PacketOut Out)
        {
            BattlefrontLogger.Trace(".");
            //Out.WriteByte((byte)GetZoneOwnership(Zones[2].ZoneId));
            //Out.WriteByte((byte)GetZoneOwnership(Zones[1].ZoneId));
            //Out.WriteByte((byte)GetZoneOwnership(Zones[0].ZoneId));
        }

        public virtual  bool PreventKillReward(ushort zoneId)
        {
            foreach (BattleFrontStatus status in ApocBattleFrontStatuses)
            {
                if ((zoneId == status.OrderZoneId || zoneId == status.DestroZoneId))
                    return status.Locked;
            }
            return false;
        }

        public virtual  float GetArtilleryDamageScale(SetRealms weaponRealm)
        {
            return 1f;
        }
    }
}