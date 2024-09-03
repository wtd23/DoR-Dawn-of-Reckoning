using Common;
using Common.Database.World.Battlefront;
using FrameWork;
using GameData;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemData;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Battlefronts.Bounty;
using WorldServer.World.Battlefronts.Keeps;
using WorldServer.World.Interfaces;
using WorldServer.World.Map;
using WorldServer.World.Objects;

namespace WorldServer.World.Battlefronts.Apocalypse
{
    public class ScalingCampaignManager : IBattleFrontManager
    {
        public static int POPULATION_BROADCAST_CHANCE = 0;
        public static int RALLY_CALL_BROADCAST_TIME_LAPSE = 6000;
        public static int RALLY_CALL_ORDER_BROADCAST_BOUNDARY = -5;
        public static int RALLY_CALL_DEST_BROADCAST_BOUNDARY = 5;

        private static readonly object LockObject = new object();
        private static readonly Logger ProgressionLogger = LogManager.GetLogger("RVRProgressionLogger");

        protected readonly EventInterface _EvtInterface = new EventInterface();

        public List<RegionMgr> RegionMgrs { get; }
        public List<rvr_progression> BattleFrontProgressions { get; }
        public List<BattleFrontStatus> BattleFrontStatuses { get; set; }
        public ImpactMatrixManager ImpactMatrixManagerInstance { get; set; }
        public BountyManager BountyManagerInstance { get; set; }
        public Dictionary<int, rvr_progression> ActiveBattlefronts { get; set; }

        public volatile int LastAAORallyCall = 0;

        public ScalingCampaignManager(List<rvr_progression> _RVRProgressions, List<RegionMgr> regionMgrs)
        {
            ActiveBattlefronts = new Dictionary<int, rvr_progression>();
            BattleFrontProgressions = _RVRProgressions;
            RegionMgrs = regionMgrs;
            BattleFrontStatuses = new List<BattleFrontStatus>();
            ImpactMatrixManagerInstance = new ImpactMatrixManager();
            BountyManagerInstance = new BountyManager();
            if (_RVRProgressions != null)
                BuildApocBattleFrontStatusList(BattleFrontProgressions);

            LastAAORallyCall = FrameWork.TCPManager.GetTimeStamp();

            _EvtInterface.AddEvent(BroadcastPlayerMessages, 600000, 0);
        }

        public void Update(long tick)
        {
            _EvtInterface.Update(tick);
        }

        private void BroadcastPlayerMessages()
        {
            ProgressionLogger.Debug($"Checking player broadcast messages... {DateTime.Now.ToString("s")} Last : {LastAAORallyCall} Now : {FrameWork.TCPManager.GetTimeStamp()}");
            foreach (pairing_infos pairing in RVRProgressionService.RVRPairings)
            {
                if (FrameWork.TCPManager.GetTimeStamp() >= RALLY_CALL_BROADCAST_TIME_LAPSE + LastAAORallyCall)
                {
                    rvr_progression rvrProg = ActiveBattlefronts[pairing.PairingId];
                    int activeBattleFrontId = ActiveBattlefronts[pairing.PairingId].BattleFrontId;
                    BattleFrontStatus activeBattleFrontStatus = GetActiveBattleFrontStatus(activeBattleFrontId);

                    LastAAORallyCall = FrameWork.TCPManager.GetTimeStamp();

                    // _aaoTracker.AgainstAllOddsMult is defined in multiples of 20 (eg 400 AAO is 20). Negative numbers means Order has AAO, Positive numbers means Dest has AAO
                    if (GetActiveCampaign(pairing.PairingId).AgainstAllOddsTracker.AgainstAllOddsMult <= RALLY_CALL_ORDER_BROADCAST_BOUNDARY)
                    {
                        foreach (var player in Player._Players)
                        {
                            if (player.Realm == SetRealms.REALMS_REALM_ORDER)
                            {
                                // Only tell players that are valid in the tier
                                if (player.ValidInTier(rvrProg.Tier, true))
                                {
                                    player.SendMessage(
                                        $"Your realm is under serious attack. Proceed with all haste to {activeBattleFrontStatus.Description}.",
                                        ChatLogFilters.CHATLOGFILTERS_C_ORDER_RVR_MESSAGE);
                                }
                            }
                        }
                    }

                    if (GetActiveCampaign(pairing.PairingId).AgainstAllOddsTracker.AgainstAllOddsMult >= RALLY_CALL_DEST_BROADCAST_BOUNDARY)
                    {
                        foreach (var player in Player._Players)
                        {
                            if (player.Realm == SetRealms.REALMS_REALM_DESTRUCTION)
                            {
                                if (player.ValidInTier(rvrProg.Tier, true))
                                {
                                    player.SendMessage(
                                        $"Your realm is under serious attack. Proceed with all haste to {activeBattleFrontStatus.Description}.",
                                        ChatLogFilters.CHATLOGFILTERS_C_DESTRUCTION_RVR_MESSAGE);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets up the Battlefront status list with default values.
        /// </summary>
        /// <param name="battleFrontProgressions"></param>
        private void BuildApocBattleFrontStatusList(List<rvr_progression> battleFrontProgressions)
        {
            lock (LockObject)
            {
                BattleFrontStatuses.Clear();

                foreach (rvr_progression _rvrProg in battleFrontProgressions)
                {
                    // Building zone objects
                    List<keep_infos> _Keeps = new List<keep_infos>();
                    List<battlefront_objectives> _BOs = new List<battlefront_objectives>();

                    // Keeps for this region.
                    // BattleFront Objectives for this region.

                    _Keeps.AddRange(BattleFrontService.GetZoneKeeps(_rvrProg.RegionId, _rvrProg.DestroZoneId));
                    _Keeps.AddRange(BattleFrontService.GetZoneKeeps(_rvrProg.RegionId, _rvrProg.OrderZoneId));
                    _BOs.AddRange(BattleFrontService.GetZoneBattlefrontObjectives(_rvrProg.RegionId, _rvrProg.DestroZoneId));
                    _BOs.AddRange(BattleFrontService.GetZoneBattlefrontObjectives(_rvrProg.RegionId, _rvrProg.OrderZoneId));

                    BattleFrontStatuses.Add(new BattleFrontStatus(ImpactMatrixManagerInstance, _rvrProg.BattleFrontId)
                    {
                        RegionId = _rvrProg.RegionId,
                        PairingId = _rvrProg.PairingId,
                        LockingRealm = (SetRealms)_rvrProg.LastOwningRealm,
                        FinalVictoryPoint = new VictoryPointProgress(_rvrProg.OrderVP, _rvrProg.DestroVP),
                        OpenTimeStamp = 0,
                        LockTimeStamp = 0,
                        Locked = true,
                        Description = _rvrProg.Description,
                        DestroZoneId = _rvrProg.DestroZoneId,
                        OrderZoneId = _rvrProg.OrderZoneId,
                        KeepList = _Keeps,
                        BattlefieldObjectives = _BOs,
                        Progression = _rvrProg
                    });
                }
            }
        }

        public bool IsPlayerRealmCaptain(uint characterId)
        {
            foreach(BattleFrontStatus status in BattleFrontStatuses)
            {
                if (status.DestructionRealmCaptain?.CharacterId == characterId ||
                   status.OrderRealmCaptain?.CharacterId == characterId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the active campaign based upon the region.
        /// </summary>
        /// <returns></returns>
        public Campaign GetActiveCampaign(int pairingId)
        {
            var activeRegionId = ActiveBattlefronts[pairingId].RegionId;
            foreach (var regionMgr in RegionMgrs)
            {
                if (regionMgr.RegionId == activeRegionId)
                {
                    return regionMgr.Campaign;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the active campaign based upon the zoneId.
        /// </summary>
        /// <returns></returns>
        public Campaign GetActiveCampaign(ushort zoneId)
        {
            foreach (BattleFrontStatus status in BattleFrontStatuses)
            {
                foreach (var regionMgr in RegionMgrs)
                {
                    if (regionMgr.RegionId == status.RegionId && (status.DestroZoneId == zoneId || status.OrderZoneId == zoneId))
                    {
                        return regionMgr.Campaign;
                    }
                }
            }
            return null;
        }

        public BattleFrontStatus GetRegionBattleFrontStatus(int regionId)
        {
            if (BattleFrontStatuses != null)
            {
                return BattleFrontStatuses.SingleOrDefault(x => x.RegionId == regionId);
            }
            else
            {
                ProgressionLogger.Debug($"Call to get region status with no statuses");
                return null;
            }
        }

        /// <summary>
        /// Log the status of all battlefronts
        /// </summary>
        public void AuditBattleFronts(int tier)
        {
            foreach (var regionMgr in RegionMgrs)
            {
                if (regionMgr.GetTier() == tier)
                {
                    foreach (var objective in regionMgr.Campaign.Objectives)
                    {
                        ProgressionLogger.Debug($"OBJECTIVE : {regionMgr.RegionName} {objective.Name} {objective.State}");
                    }
                    foreach (var keep in regionMgr.Campaign.Keeps)
                    {
                        ProgressionLogger.Debug($"KEEP : {regionMgr.RegionName} {keep.Info.Name} {keep.KeepStatus} ");
                        foreach (var keepDoor in keep.Doors)
                        {
                            ProgressionLogger.Debug($"++ DOOR : {keep.Info.Name} {keepDoor.Info.Number} {keepDoor.GameObject?.PctHealth} {keepDoor.GameObject?.IsAttackable}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lock Battlefronts across all the regions.
        /// </summary>
        public void LockBattleFrontsAllRegions(int tier, bool forceDefaultRealm = false)
        {
            foreach (pairing_infos campaign in RVRProgressionService.RVRPairings)
            {
                foreach (var regionMgr in RegionMgrs)
                {
                    if (regionMgr.GetTier() == tier)
                    {
                        // Find and update the status of the battlefront status (list of BFStatuses is only for this Tier)
                        foreach (var apocBattleFrontStatus in BattleFrontStatuses)
                        {
                            apocBattleFrontStatus.Locked = true;
                            apocBattleFrontStatus.OpenTimeStamp = FrameWork.TCPManager.GetTimeStamp();
                            // Determine what the "start" realm this battlefront should be locked to.
                            if (forceDefaultRealm)
                                apocBattleFrontStatus.LockingRealm = (SetRealms)BattleFrontProgressions.Single(x => x.BattleFrontId == apocBattleFrontStatus.BattleFrontId).DefaultRealmLock;
                            else
                            {
                                apocBattleFrontStatus.LockingRealm = (SetRealms)BattleFrontProgressions.Single(x => x.BattleFrontId == apocBattleFrontStatus.BattleFrontId).LastOwningRealm;
                            }
                            apocBattleFrontStatus.FinalVictoryPoint = new VictoryPointProgress();
                            apocBattleFrontStatus.LockTimeStamp = FrameWork.TCPManager.GetTimeStamp();
                        }

                        if (regionMgr.Campaign == null)
                            continue;

                        regionMgr.Campaign.VictoryPointProgress = new VictoryPointProgress();

                        foreach (var objective in regionMgr.Campaign.Objectives)
                        {
                            if (forceDefaultRealm)
                            {
                                objective.OwningRealm = (SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].DefaultRealmLock;
                            }
                            else
                            {
                                objective.OwningRealm = (SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].LastOwningRealm;
                            }
                            //objective.fsm.Fire(CampaignObjectiveStateMachine.Command.OnLockZone);
                            objective.SetObjectiveLocked();
                            ProgressionLogger.Debug($" Locking BattlefieldObjective to {(SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].LastOwningRealm} {objective.Name} {objective.State} {objective.State}");
                        }

                        foreach (var keep in regionMgr.Campaign.Keeps)
                        {
                            if (forceDefaultRealm)
                            {
                                keep.PendingRealm = (SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].DefaultRealmLock;
                            }
                            else
                            {
                                keep.PendingRealm = (SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].LastOwningRealm;
                            }
                            //keep.fsm.Fire(SM.Command.OnLockZone);
                            keep.SetKeepLocked();
                            ProgressionLogger.Debug($" Locking Keep {keep.Info.Name} to {(SetRealms)regionMgr.Campaign.BattleFrontManager.ActiveBattlefronts[campaign.PairingId].LastOwningRealm} {keep.KeepStatus} ");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Open the active battlefront (which has been set in this class [ActiveBattleFront]).
        /// Reset the VPP for the active battlefront.
        /// </summary>
        /// <returns></returns>
        public void OpenActiveBattlefront()
        {
            foreach (pairing_infos campaign in RVRProgressionService.RVRPairings)
            {
                rvr_progression activeBattlefront = ActiveBattlefronts[campaign.PairingId];
                try
                {
                    var activeRegion = RegionMgrs.Single(x => x.RegionId == activeBattlefront.RegionId);
                    ProgressionLogger.Info($"Opening Active battlefront in {activeRegion.RegionName} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");

                    activeRegion.Campaign.VictoryPointProgress.Reset(activeRegion.Campaign);
                    ProgressionLogger.Info($"Resetting VP Progress {activeRegion.RegionName} BF Id : {activeBattlefront.BattleFrontId} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");

                    // Find and update the status of the battlefront status.
                    foreach (var apocBattleFrontStatus in BattleFrontStatuses)
                    {
                        if (apocBattleFrontStatus.BattleFrontId == activeBattlefront.BattleFrontId)
                        {
                            lock (apocBattleFrontStatus)
                            {
                                ProgressionLogger.Info(
                                    $"Resetting BFStatus {activeRegion.RegionName} BF Id : {activeBattlefront.BattleFrontId} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");

                                apocBattleFrontStatus.Locked = false;
                                apocBattleFrontStatus.OpenTimeStamp = FrameWork.TCPManager.GetTimeStamp();
                                apocBattleFrontStatus.LockingRealm = SetRealms.REALMS_REALM_NEUTRAL;
                                apocBattleFrontStatus.FinalVictoryPoint = new VictoryPointProgress();
                                apocBattleFrontStatus.LockTimeStamp = 0;

                                // Reset the population for the battle front status
                                ProgressionLogger.Info(
                                    $"InitializePopulationList {activeRegion.RegionName} BF Id : {activeBattlefront.BattleFrontId} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");
                                GetActiveCampaign(campaign.PairingId).InitializePopulationList(activeBattlefront.BattleFrontId);

                                //GetActiveCampaign().StartWanderingMobs(ActiveBattleFront.ZoneId);
                            }
                        }
                    }

                    if (activeRegion.Campaign == null)
                    {
                        ProgressionLogger.Info($"activeRegion.Campaign is null");
                        return;
                    }

                    if (activeRegion.Campaign.Objectives == null)
                    {
                        ProgressionLogger.Warn($"activeRegion.Campaign (objectives) is null");
                        return;
                    }

                    ProgressionLogger.Info($"Unlocking objectives {activeRegion.RegionName} BF Id : {activeBattlefront.BattleFrontId} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");
                    foreach (var flag in activeRegion.Campaign.Objectives)
                    {
                        if (activeBattlefront.DestroZoneId == flag.ZoneId)
                        {
                            flag.OpenBattleFront();
                            //flag.SetObjectiveSafe();
                        }
                    }
                    foreach (var flag in activeRegion.Campaign.Objectives)
                    {
                        if (activeBattlefront.OrderZoneId == flag.ZoneId)
                        {
                            flag.OpenBattleFront();
                            //flag.SetObjectiveSafe();
                        }
                    }
                    if (activeRegion.Campaign.Keeps == null)
                    {
                        ProgressionLogger.Warn($"activeRegion.Campaign (keeps) is null");
                        return;
                    }

                    ProgressionLogger.Info($"Unlocking keeps {activeRegion.RegionName} BF Id : {activeBattlefront.BattleFrontId} Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");
                    foreach (var keep in activeRegion.Campaign.Keeps)
                    {
                        if (activeBattlefront.DestroZoneId == keep.ZoneId)
                        {
                            ProgressionLogger.Debug($"Informing Keep of Open battlefront Name : {keep.Info.Name} Zone : {keep.ZoneId} ");
                            keep.OpenBattleFront();
                        }
                    }
                    foreach (var keep in activeRegion.Campaign.Keeps)
                    {
                        if (activeBattlefront.OrderZoneId == keep.ZoneId)
                        {
                            ProgressionLogger.Debug($"Informing Keep of Open battlefront Name : {keep.Info.Name} Zone : {keep.ZoneId} ");
                            keep.OpenBattleFront();
                        }
                    }
                    activeRegion.Campaign.DestructionDominationCounter = Core.Config.DestructionDominationTimerLength;
                    activeRegion.Campaign.OrderDominationCounter = Core.Config.OrderDominationTimerLength;
                }


                catch (Exception e)
                {
                    ProgressionLogger.Error($"Exception. Destro Zone : {activeBattlefront.DestroZoneId}, Order Zone : {activeBattlefront.OrderZoneId} {activeBattlefront.Description}");
                    throw;
                }
            }
        }

        public void LockActiveBattleFront(int pairingId, SetRealms realm, int forceNumberBags = 0)
        {
            rvr_progression activeBf = ActiveBattlefronts[pairingId];
            var activeRegion = RegionMgrs.Single(x => x.RegionId == activeBf.RegionId);
            ProgressionLogger.Info($" Locking battlefront in {activeRegion.RegionName} Destro Zone : {activeBf.DestroZoneId}, Order Zone : {activeBf.OrderZoneId} {activeBf.Description}");

            LockBattleFrontStatus(activeBf.BattleFrontId, realm, activeRegion.Campaign.VictoryPointProgress);

            foreach (var flag in activeRegion.Campaign.Objectives)
            {
                if (activeBf.DestroZoneId == flag.ZoneId)
                {
                    flag.OwningRealm = realm;
                    flag.SetObjectiveLocked();
                }
            }

            foreach (BattleFrontKeep keep in activeRegion.Campaign.Keeps)
            {
                if (activeBf.DestroZoneId == keep.ZoneId)
                    keep.OnLockZone(realm);
            }

            foreach (var flag in activeRegion.Campaign.Objectives)
            {
                if (activeBf.OrderZoneId == flag.ZoneId)
                {
                    flag.OwningRealm = realm;
                    flag.SetObjectiveLocked();
                }
            }

            foreach (BattleFrontKeep keep in activeRegion.Campaign.Keeps)
            {
                if (activeBf.OrderZoneId == keep.ZoneId)
                    keep.OnLockZone(realm);
            }
            ProgressionLogger.Debug($"Removing any siege from active region");
            // Destroy any active siege in this zone.
            try
            {
                var siegeInRegion = activeRegion?.Objects.Where(x => x is Siege);
                foreach (var siege in siegeInRegion)
                {
                    if (siege is Siege)
                    {
                        ProgressionLogger.Debug($"Calling Destroy on {siege.Name}");
                        siege.Destroy();
                    }
                }
            }
            catch (Exception e)
            {
                ProgressionLogger.Debug($"{e.Message}{e.StackTrace}");
            }

            activeRegion.Campaign.LockBattleFront(activeBf, realm, forceNumberBags);
        }

        public void LockBattleFrontStatus(int battleFrontId, SetRealms lockingRealm, VictoryPointProgress vpp)
        {
            var activeStatus = BattleFrontStatuses.Single(x => x.BattleFrontId == battleFrontId);

            if (activeStatus == null)
                ProgressionLogger.Warn($"Could not locate Active Status for battlefront Id {battleFrontId}");

            activeStatus.Locked = true;
            activeStatus.LockingRealm = lockingRealm;
            activeStatus.FinalVictoryPoint = vpp;
            activeStatus.LockTimeStamp = FrameWork.TCPManager.GetTimeStamp();

            ProgressionLogger.Info($"Locking BF Status {activeStatus.Description} to realm:{lockingRealm}");
        }

        public BattleFrontStatus GetActiveBattleFrontStatus(int battleFrontId)
        {
            return BattleFrontStatuses.Single(x => x.BattleFrontId == battleFrontId);
        }

        public List<BattleFrontStatus> GetBattleFrontStatusList()
        {
            return BattleFrontStatuses;
        }

        public bool IsBattleFrontLocked(int battleFrontId)
        {
            foreach (var ApocBattleFrontStatus in BattleFrontStatuses)
            {
                if (ApocBattleFrontStatus.BattleFrontId == battleFrontId)
                {
                    return ApocBattleFrontStatus.Locked;
                }
            }
            return false;
        }

        public BattleFrontStatus GetBattleFrontStatus(int battleFrontId)
        {
            try
            {
                return BattleFrontStatuses.Single(x => x.BattleFrontId == battleFrontId);
            }
            catch (Exception e)
            {
                ProgressionLogger.Warn($"Battlefront Id : {battleFrontId} Exception : {e.Message} ");
                throw;
            }
        }

        public bool IsInActiveBattlefront(ushort? zoneId)
        {
            return BattleFrontStatuses.Where(x => (x.DestroZoneId == zoneId || x.OrderZoneId == zoneId) && !x.Locked).FirstOrDefault() != null;
        }

        /// <summary>
        /// Get the ActiveBattleFront to be Praag if no zones are marked open in rvr_progression, otherwise use the LastOpenedZone
        /// </summary>
        public void GetActiveBattleFrontsFromProgression()
        {
            ProgressionLogger.Debug($" Getting battlefront progression...");
            foreach (pairing_infos campaign in RVRProgressionService.RVRPairings)
            {
                var list = BattleFrontProgressions.Select(x => x).Where(x => x.LastOpenedZone == 1 && x.PairingId == campaign.PairingId).ToList();
                rvr_progression bf = list.FirstOrDefault();
                if (ActiveBattlefronts.ContainsKey(bf.PairingId))
                {
                    ActiveBattlefronts.Remove(bf.PairingId);
                }
                ActiveBattlefronts.Add(bf.PairingId, bf);
                ProgressionLogger.Debug($"Campaign: {campaign.PairingId} Active : {bf.Description}");
            }
        }
        /// <summary>
        /// Get the ActiveBattleFront to be Praag if no zones are marked open in rvr_progression, otherwise use the LastOpenedZone
        /// </summary>
        public rvr_progression GetActiveBattleFrontFromProgression(int pairing)
        {
            ProgressionLogger.Debug($" Getting battlefront progression...");
            pairing_infos campaign = RVRProgressionService.RVRPairings.Where(x => x.PairingId == pairing).FirstOrDefault();
            if (campaign != null)
            {
                var list = BattleFrontProgressions.Select(x => x).Where(x => x.LastOpenedZone == 1 && x.PairingId == campaign.PairingId).ToList();
                rvr_progression bf = list.FirstOrDefault();
                if (ActiveBattlefronts.ContainsKey(bf.PairingId))
                {
                    ActiveBattlefronts.Remove(bf.PairingId);
                }
                ActiveBattlefronts.Add(bf.PairingId, bf);
                ProgressionLogger.Debug($"Campaign: {campaign.PairingId} Active : {bf.Description}");
                return bf;
            }
            return null;
        }

        public rvr_progression GetBattleFrontByName(string name)
        {
            return BattleFrontProgressions.Single(x => x.Description.Contains(name));
        }

        public rvr_progression GetBattleFrontByBattleFrontId(int id)
        {
            return BattleFrontProgressions.Single(x => x.BattleFrontId == id);
        }


        /// <summary>
        /// Given a realm that locked the current Active Battlefront, find the next Battlefront.
        /// </summary>
        public rvr_progression AdvanceBattleFront(int pairingid, SetRealms lockingRealm)
        {
            rvr_progression activeBattleFront = ActiveBattlefronts[pairingid];
            if (lockingRealm == SetRealms.REALMS_REALM_ORDER)
            {
                if (activeBattleFront.OrderWinProgression != 0)
                {
                    var newBattleFront = GetBattleFrontByBattleFrontId(activeBattleFront.OrderWinProgression);
                    ProgressionLogger.Debug($"Order Win : Advancing Battlefront from {activeBattleFront.Description} to {newBattleFront.Description}");
                    ActiveBattlefronts.Remove(pairingid);
                    ActiveBattlefronts.Add(pairingid, newBattleFront);
                    return newBattleFront;
                }
                else
                {
                    return null;
                }
            }

            if (lockingRealm == SetRealms.REALMS_REALM_DESTRUCTION)
            {
                if (activeBattleFront.DestWinProgression != 0)
                {
                    var newBattleFront = GetBattleFrontByBattleFrontId(activeBattleFront.DestWinProgression);
                    ProgressionLogger.Debug($"Destruction Win : Advancing Battlefront from {activeBattleFront.Description} to {newBattleFront.Description}");
                    ActiveBattlefronts.Remove(pairingid);
                    ActiveBattlefronts.Add(pairingid, newBattleFront);
                    return newBattleFront;
                }
                else
                {
                    return null;
                }
            }

            return GetActiveBattleFrontFromProgression(pairingid);
        }

        public void UpdateRVRPRogression(SetRealms lockingRealm, rvr_progression oldProg, rvr_progression newProg)
        {
            oldProg.DestroVP = oldProg.OrderVP = 0;
            oldProg.LastOpenedZone = 0;
            oldProg.LastOwningRealm = (byte)lockingRealm;

            newProg.LastOwningRealm = 0;
            newProg.LastOpenedZone = 1;
        }

        public BattleFrontStatus GetActiveCampaign(RegionMgr region)
        {
            foreach(BattleFrontStatus status in BattleFrontStatuses)
            {
                if (status.RegionId == region.RegionId)
                    return status;
            }
            return null;
        }

        public bool IsInContestedZone(ushort zoneId)
        {
            foreach (BattleFrontStatus status in BattleFrontStatuses)
            {
                if (!status.Locked && (status.DestroZoneId == zoneId || status.OrderZoneId == zoneId))
                    return true;
            }
            return false;
        }

        [ConsoleHandler("battlefront-audit", 1, "Audit Battlefront <Tier>")]
        public class AuditBattleFrontConsole : IConsoleHandler
        {
            public bool HandleCommand(string command, List<string> args)
            {
                string tier = args[0];

                WorldMgr.ScalingCampaignManager.AuditBattleFronts(Convert.ToInt32(tier));

                return true;
            }
        }

        public byte GetBattleFrontState(int pairing, int tier)
        {
            return 2;
        }
    }
}