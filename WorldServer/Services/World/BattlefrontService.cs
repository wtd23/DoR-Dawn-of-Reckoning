using Common;
using Common.Database.World.Battlefront;
using Common.Database.World.BattleFront;
using FrameWork;
using GameData;
using System.Collections.Generic;
using System.Linq;
using WorldServer.World.Positions;

namespace WorldServer.Services.World
{
    [Service]
    public class BattleFrontService : ServiceBase
    {
        [LoadingFunction(true)]
        public static void LoadBattleFront()
        {
            BattleFrontStatus = Database.SelectAllObjects<battlefront_status>().ToDictionary(g => g.RegionId);

            LoadBattleFrontGuards();
            LoadResourceSpawns();

            LoadBattleFrontObjectives();
            LoadBattleFrontObjects();

            LoadKeepInfos();
            LoadPlayerKeepSpawnPoints();
        }

        #region Objectives

        public static Dictionary<uint, List<battlefront_objectives>> _BattleFrontObjectives = new Dictionary<uint, List<battlefront_objectives>>();

        private static void LoadBattleFrontObjectives()
        {
            _BattleFrontObjectives = new Dictionary<uint, List<battlefront_objectives>>();

            Log.Debug("WorldMgr", "Loading BattleFront_Objectives...");

            IList<battlefront_objectives> Objectives = Database.SelectAllObjects<battlefront_objectives>();

            int Count = 0;
            foreach (battlefront_objectives Obj in Objectives)
            {
                if (!_BattleFrontObjectives.ContainsKey(Obj.RegionId))
                    _BattleFrontObjectives.Add(Obj.RegionId, new List<battlefront_objectives>());

                _BattleFrontObjectives[Obj.RegionId].Add(Obj);

                if (_BattleFrontGuards.ContainsKey(Obj.Entry))
                    Obj.Guards = _BattleFrontGuards[Obj.Entry];

                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Campaign Objectives");
        }

        public static List<battlefront_objectives> GetBattleFrontObjectives(uint RegionId)
        {
            if (_BattleFrontObjectives.ContainsKey(RegionId))
            {
                return _BattleFrontObjectives[RegionId];
            }

            return null;
        }

        #endregion Objectives

        #region Keeps

        public static Dictionary<uint, List<keep_infos>> _KeepInfos = new Dictionary<uint, List<keep_infos>>();

        public static void LoadKeepInfos()
        {
            LoadKeepCreatures();
            LoadKeepDoors();
            LoadKeepSiegeSpawnPoints();

            _KeepInfos = new Dictionary<uint, List<keep_infos>>();

            Log.Debug("WorldMgr", "Loading BattleFront_Objectives...");

            IList<keep_infos> keepInfos = Database.SelectAllObjects<keep_infos>();

            int Count = 0;
            foreach (keep_infos keepInfo in keepInfos)
            {
                if (!_KeepInfos.ContainsKey(keepInfo.RegionId))
                    _KeepInfos.Add(keepInfo.RegionId, new List<keep_infos>());

                _KeepInfos[keepInfo.RegionId].Add(keepInfo);

                if (_KeepCreatures.ContainsKey(keepInfo.KeepId))
                    keepInfo.Creatures = _KeepCreatures[keepInfo.KeepId];

                if (_KeepDoors.ContainsKey(keepInfo.KeepId))
                    keepInfo.Doors = _KeepDoors[keepInfo.KeepId];

                if (_KeepSiegeSpawnPoints.ContainsKey(keepInfo.KeepId))
                    keepInfo.KeepSiegeSpawnPoints = _KeepSiegeSpawnPoints[keepInfo.KeepId];
                else
                    Log.Error("WorldMgr", "No spawnpoints found for " + keepInfo.Name);

                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Keep Infos");
        }

        public static Dictionary<int, List<keep_creatures>> _KeepCreatures = new Dictionary<int, List<keep_creatures>>();

        public static void LoadKeepCreatures()
        {
            _KeepCreatures = new Dictionary<int, List<keep_creatures>>();

            Log.Debug("WorldMgr", "Loading Keep_Creatures...");

            IList<keep_creatures> Creatures = Database.SelectAllObjects<keep_creatures>();

            int Count = 0;
            foreach (keep_creatures Creature in Creatures)
            {
                if (!_KeepCreatures.ContainsKey(Creature.KeepId))
                    _KeepCreatures.Add(Creature.KeepId, new List<keep_creatures>());

                _KeepCreatures[Creature.KeepId].Add(Creature);
                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Keep Creatures");
        }

        public static Dictionary<int, List<keep_doors>> _KeepDoors = new Dictionary<int, List<keep_doors>>();

        public static void LoadKeepDoors()
        {
            _KeepDoors = new Dictionary<int, List<keep_doors>>();

            Log.Debug("WorldMgr", "Loading Keep_Doors...");

            IList<keep_doors> Doors = Database.SelectAllObjects<keep_doors>();

            int Count = 0;
            foreach (keep_doors Door in Doors)
            {
                if (!_KeepDoors.ContainsKey(Door.KeepId))
                    _KeepDoors.Add(Door.KeepId, new List<keep_doors>());

                _KeepDoors[Door.KeepId].Add(Door);
                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Keep Doors");
        }

        public static Dictionary<int, List<keep_spawn_points>> _KeepSiegeSpawnPoints = new Dictionary<int, List<keep_spawn_points>>();

        public static void LoadKeepSiegeSpawnPoints()
        {
            _KeepSiegeSpawnPoints = new Dictionary<int, List<keep_spawn_points>>();

            Log.Debug("WorldMgr", "Loading KeepSiegeSpawnPoints...");

            IList<keep_spawn_points> points = Database.SelectAllObjects<keep_spawn_points>();

            int Count = 0;
            foreach (keep_spawn_points point in points)
            {
                if (!_KeepSiegeSpawnPoints.ContainsKey(point.KeepId))
                    _KeepSiegeSpawnPoints.Add(point.KeepId, new List<keep_spawn_points>());

                _KeepSiegeSpawnPoints[point.KeepId].Add(point);
                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " KeepSiegeSpawnPoints");
        }

        public static Dictionary<int, player_keep_spawn> _PlayerKeepSpawnPoints = new Dictionary<int, player_keep_spawn>();

        public static void LoadPlayerKeepSpawnPoints()
        {
            _PlayerKeepSpawnPoints = new Dictionary<int, player_keep_spawn>();

            Log.Debug("WorldMgr", "Loading PlayerKeepSpawn...");

            var spawns = Database.SelectAllObjects<player_keep_spawn>();

            int count = 0;
            foreach (var spawn in spawns)
            {
                if (!_PlayerKeepSpawnPoints.ContainsKey(spawn.KeepId))
                    _PlayerKeepSpawnPoints.Add(spawn.KeepId, new player_keep_spawn());

                _PlayerKeepSpawnPoints[spawn.KeepId] = spawn;
                ++count;
            }

            Log.Success("WorldMgr", "Loaded " + count + " Player Keep Spawn Points");
        }

        public static List<keep_infos> GetKeepInfos(uint RegionId)
        {
            if (_KeepInfos.ContainsKey(RegionId))
            {
                return _KeepInfos[RegionId];
            }

            return null;
        }

        #endregion Keeps

        #region Guards

        public static Dictionary<int, List<battlefront_guards>> _BattleFrontGuards = new Dictionary<int, List<battlefront_guards>>();

        public static void LoadBattleFrontGuards()
        {
            _BattleFrontGuards = new Dictionary<int, List<battlefront_guards>>();

            Log.Debug("WorldMgr", "Loading BattleFront_Guards...");

            IList<battlefront_guards> Guards = Database.SelectAllObjects<battlefront_guards>();

            int Count = 0;
            foreach (battlefront_guards Guard in Guards)
            {
                if (!_BattleFrontGuards.ContainsKey(Guard.ObjectiveId))
                    _BattleFrontGuards.Add(Guard.ObjectiveId, new List<battlefront_guards>());

                _BattleFrontGuards[Guard.ObjectiveId].Add(Guard);
                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Campaign Guards");
        }

        #endregion Guards

        #region Resources

        public static Dictionary<int, List<battlefront_resource_spawns>> ResourceSpawns = new Dictionary<int, List<battlefront_resource_spawns>>();

        private static void LoadResourceSpawns()
        {
            ResourceSpawns = new Dictionary<int, List<battlefront_resource_spawns>>();

            Log.Debug("WorldMgr", "Loading Resource Spawns...");

            IList<battlefront_resource_spawns> resSpawns = Database.SelectAllObjects<battlefront_resource_spawns>();

            int count = 0;

            foreach (battlefront_resource_spawns res in resSpawns)
            {
                if (!ResourceSpawns.ContainsKey(res.Entry))
                    ResourceSpawns.Add(res.Entry, new List<battlefront_resource_spawns>());

                ResourceSpawns[res.Entry].Add(res);
                ++count;
            }

            Log.Success("WorldMgr", "Loaded " + count + " resource points.");
        }

        public static List<battlefront_resource_spawns> GetResourceSpawns(int objectiveId)
        {
            return ResourceSpawns.ContainsKey(objectiveId) ? ResourceSpawns[objectiveId] : null;
        }

        #endregion Resources

        #region RvRObjects

        public static List<rvr_objects> RvRObjects;

        [LoadingFunction(true)]
        public static void LoadRvRObjects()
        {
            Log.Debug("WorldMgr", "Loading Creature_Protos...");

            RvRObjects = Database.SelectAllObjects<rvr_objects>().ToList();

            Log.Success("LoadRvRObjects", "Loaded " + RvRObjects.Count + " RvR Objects");
        }

        public static rvr_objects GetRvRObjectInfo(int index)
        {
            if (index < RvRObjects.Count)
                return RvRObjects[index];
            return null;
        }

        #endregion RvRObjects

        #region Generic Campaign objects

        /// <summary>
        /// Arrays of warcamp entrances (0/1 for order/destro) indexed by zone id
        /// This must be changed to private later
        /// </summary>
        public static Dictionary<ushort, Point3D[]> _warcampEntrances;

        /// <summary>Arrays of portals to warcamp indexed by zone id and objective ID</summary>
        private static Dictionary<ushort, Dictionary<int, battlefront_objects>> _portalsToWarcamp;

        /// <summary>Arrays of portals to warcamp indexed by zone id, realm (0/1 for order/destro) and objective ID</summary>
        private static Dictionary<ushort, Dictionary<int, battlefront_objects>[]> _portalsToObjective;

        /// <summary>
        /// Loads BattleFront_objects table.
        /// </summary>
        /// <remarks>Public for gm commands</remarks>
        public static void LoadBattleFrontObjects()
        {
            _warcampEntrances = new Dictionary<ushort, Point3D[]>();
            _portalsToWarcamp = new Dictionary<ushort, Dictionary<int, battlefront_objects>>();
            _portalsToObjective = new Dictionary<ushort, Dictionary<int, battlefront_objects>[]>();

            Log.Debug("WorldMgr", "Loading Campaign objects...");

            IList<battlefront_objects> objects = Database.SelectAllObjects<battlefront_objects>();

            int count = 0;

            foreach (battlefront_objects res in objects)
            {
                switch (res.Type)
                {
                    case (ushort)battlefront_object_type.WARCAMP_ENTRANCE:
                        // Entrances to warcamp necessary to compute objective rewards and spawn farm check
                        if (!_warcampEntrances.ContainsKey(res.ZoneId))
                            _warcampEntrances.Add(res.ZoneId, new Point3D[2]);

                        _warcampEntrances[res.ZoneId][res.Realm - 1] = new Point3D(res.X, res.Y, res.Z);
                        break;

                    case (ushort)battlefront_object_type.WARCAMP_PORTAL:
                        // Objective -> warcamp portals
                        if (!_portalsToWarcamp.ContainsKey(res.ZoneId))
                            _portalsToWarcamp.Add(res.ZoneId, new Dictionary<int, battlefront_objects>());

                        _portalsToWarcamp[res.ZoneId].Add(res.ObjectiveID, res);
                        break;

                    case (ushort)battlefront_object_type.OBJECTIVE_PORTAL:
                        // Warcamp -> objective portals
                        if (!_portalsToObjective.ContainsKey(res.ZoneId))
                        {
                            _portalsToObjective.Add(res.ZoneId, new Dictionary<int, battlefront_objects>[] {
                                    new Dictionary<int, battlefront_objects>(),
                                    new Dictionary<int, battlefront_objects>(),
                                });
                        }

                        _portalsToObjective[res.ZoneId][res.Realm - 1].Add(res.ObjectiveID, res);
                        break;

                    default:
                        Log.Error("WorldMgr", "Unkown type for object : " + res.Type.ToString());
                        break;
                }
                ++count;
            }

            Log.Success("WorldMgr", "Loaded " + count + " Campaign objects.");
        }

        /// <summary>
        /// Gets the warcamp entrance in a zone for given realm.
        /// </summary>
        /// <param name="zoneId">Zone identifer</param>
        /// <param name="realm">Order/destro</param>
        /// <returns>Warcamp entrance coordinate or null if does not exists or is not parameterized
        /// (given zone's inner coordinates)</returns>
        public static Point3D GetWarcampEntrance(ushort zoneId, SetRealms realm)
        {
            if (_warcampEntrances.ContainsKey(zoneId))
                return _warcampEntrances[zoneId][(int)realm - 1];
            return null;
        }

        /// <summary>
        /// Gets the portal to warcamp for the given battlefield objective.
        /// </summary>
        /// <param name="zoneId">Zone identifer</param>
        /// <param name="objectiveId">Objective identifier</param>
        /// <returns>Portal, null if not found</returns>
        public static battlefront_objects GetPortalToWarcamp(ushort zoneId, int objectiveId)
        {
            if (_portalsToWarcamp.ContainsKey(zoneId))
            {
                Dictionary<int, battlefront_objects> zoneObjects = _portalsToWarcamp[zoneId];
                if (zoneObjects.ContainsKey(objectiveId))
                    return zoneObjects[objectiveId];
            }
            return null;
        }

        /// <summary>
        /// Gets the portal to the given battlefield objective for given realm.
        /// </summary>
        /// <param name="zoneId">Zone identifer</param>
        /// <param name="objectiveId">Objective identifier</param>
        /// <param name="realm">From order/destro warcamp</param>
        /// <returns>List of portals indexed by objective ID, null if not found</returns>
        public static battlefront_objects GetPortalToObjective(ushort zoneId, int objectiveId, SetRealms realm)
        {
            if (_portalsToObjective.ContainsKey(zoneId))
            {
                Dictionary<int, battlefront_objects>[] zoneObjects = _portalsToObjective[zoneId];
                if (zoneObjects[(int)realm - 1].ContainsKey(objectiveId))
                    return _portalsToObjective[zoneId][(int)realm - 1][objectiveId];
            }
            return null;
        }

        #endregion Generic Campaign objects

        #region Status - Updated at runtime

        public static Dictionary<int, battlefront_status> BattleFrontStatus;

        public static battlefront_status GetStatusFor(int regionId)
        {
            lock (BattleFrontStatus)
            {
                if (BattleFrontStatus.ContainsKey(regionId))
                    return BattleFrontStatus[regionId];

                BattleFrontStatus.Add(regionId, new battlefront_status(regionId));
            }

            Database.AddObject(BattleFrontStatus[regionId]);

            return BattleFrontStatus[regionId];
        }

        #endregion Status - Updated at runtime

        public static List<keep_infos> GetZoneKeeps(int regionId, int zoneId)
        {
            return (from keyValuePair in _KeepInfos.Where(x => x.Key == regionId)
                    from keep in keyValuePair.Value
                    where keep.ZoneId == zoneId
                    select keep).ToList();
        }

        public static List<battlefront_objectives> GetZoneBattlefrontObjectives(int regionId, int zoneId)
        {
            return (from keyValuePair in _BattleFrontObjectives.Where(x => x.Key == regionId)
                    from bo
                    in keyValuePair.Value
                    where bo.ZoneId == zoneId
                    select bo).ToList();
        }

        public static void SetCampaignBuff(int buffId, int battleFrontId)
        {
        }
    }
}