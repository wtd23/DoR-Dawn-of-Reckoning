using Common;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.World.Objects;
using WorldServer.World.Objects.Instances;

namespace WorldServer.Services.World
{
    [Service(typeof(CreatureService), typeof(GameObjectService), typeof(ItemService))]
    public class InstanceService : ServiceBase
    {
        public static Dictionary<uint, List<instance_creature_spawns>> _InstanceSpawns;
        public static Dictionary<uint, List<instance_boss_spawns>> _InstanceBossSpawns;
        public static Dictionary<uint, instance_infos> _InstanceInfo;
        public static Dictionary<uint, List<instance_encounters>> _InstanceEncounter;
        public static Dictionary<string, instance_lockouts> _InstanceLockouts;
        public static Dictionary<string, instance_statistics> _InstanceStatistics;

        #region loading methods

        [LoadingFunction(true)]
        public static void LoadInstance_Creatures()
        {
            _InstanceSpawns = new Dictionary<uint, List<instance_creature_spawns>>();

            IList<instance_creature_spawns> InstanceSpawns = Database.SelectAllObjects<instance_creature_spawns>();

            foreach (instance_creature_spawns Obj in InstanceSpawns)
            {
                List<instance_creature_spawns> Objs;
                if (!_InstanceSpawns.TryGetValue(Obj.ZoneID, out Objs))
                {
                    Objs = new List<instance_creature_spawns>();
                    _InstanceSpawns.Add(Obj.ZoneID, Objs);
                }

                Objs.Add(Obj);
            }
            Log.Success("WorldMgr", "Loaded " + _InstanceSpawns.Count + "Instance_Spawn");
        }

        [LoadingFunction(true)]
        public static void LoadInstance_Boss_Creatures()
        {
            _InstanceBossSpawns = new Dictionary<uint, List<instance_boss_spawns>>();

            IList<instance_boss_spawns> InstanceSpawns = Database.SelectAllObjects<instance_boss_spawns>();

            foreach (instance_boss_spawns Obj in InstanceSpawns)
            {
                List<instance_boss_spawns> Objs;
                if (!_InstanceBossSpawns.TryGetValue(Obj.InstanceID, out Objs))
                {
                    Objs = new List<instance_boss_spawns>();
                    _InstanceBossSpawns.Add(Obj.InstanceID, Objs);
                }

                Objs.Add(Obj);
            }
            Log.Success("WorldMgr", "Loaded " + _InstanceBossSpawns.Count + "Instance_Boss_Spawn");
        }

        [LoadingFunction(true)]
        public static void LoadInstance_Info()
        {
            _InstanceInfo = new Dictionary<uint, instance_infos>();

            IList<instance_infos> InstanceInfo = Database.SelectAllObjects<instance_infos>();

            foreach (instance_infos II in InstanceInfo)
            {
                _InstanceInfo.Add(II.ZoneID, II);
            }
            Log.Success("WorldMgr", "Loaded " + _InstanceInfo.Count + "Instance_Info");
        }

        [LoadingFunction(true)]
        public static void LoadInstance_Lockouts()
        {
            _InstanceLockouts = new Dictionary<string, instance_lockouts>();

            IList<instance_lockouts> InstanceLockouts = Database.SelectAllObjects<instance_lockouts>();

            foreach (instance_lockouts Obj in InstanceLockouts)
            {
                _InstanceLockouts.Add(Obj.InstanceID, Obj);
            }

            Log.Success("WorldMgr", "Loaded " + _InstanceLockouts.Count + "Instance_Lockouts");
        }

        [LoadingFunction(true)]
        public static void LoadInstance_Encounter()
        {
            _InstanceEncounter = new Dictionary<uint, List<instance_encounters>>();

            IList<instance_encounters> InstanceEncounter = Database.SelectAllObjects<instance_encounters>();

            foreach (instance_encounters Obj in InstanceEncounter)
            {
                List<instance_encounters> Objs;
                if (!_InstanceEncounter.TryGetValue(Obj.InstanceID, out Objs))
                {
                    Objs = new List<instance_encounters>();
                    _InstanceEncounter.Add(Obj.InstanceID, Objs);
                }

                Objs.Add(Obj);
            }
            Log.Success("WorldMgr", "Loaded " + _InstanceEncounter.Count + "Instance_Encounters");
        }

        [LoadingFunction(true)]
        public static void LoadInstances_Statistics()
        {
            _InstanceStatistics = new Dictionary<string, instance_statistics>();

            IList<instance_statistics> InstanceStatistics = Database.SelectAllObjects<instance_statistics>();

            foreach (instance_statistics Obj in InstanceStatistics)
            {
                //_InstanceStatistics.Add(Obj.InstanceID, Obj);
                Obj.Dirty = true;
                Database.DeleteObject(Obj);
            }
            Database.ForceSave();

            Log.Success("WorldMgr", "Loaded " + _InstanceStatistics.Count + "Instances_Statistics");
        }

        #endregion loading methods

        #region access methods

        private static instance_statistics AddNewInstanceStatisticsEntry(string instanceID)
        {
            instance_statistics stat = new instance_statistics()
            {
                InstanceID = instanceID,
                lockouts_InstanceID = string.Empty,
                playerIDs = string.Empty,
                ttkPerBoss = string.Empty,
                deathCountPerBoss = string.Empty,
                attemptsPerBoss = string.Empty
            };
            _InstanceStatistics.Add(instanceID, stat);

            stat.Dirty = true;
            Database.AddObject(stat);
            return stat;
        }

        public static void SaveLockoutInstanceID(string instanceID, instance_lockouts lockout)
        {
            if (lockout == null)
                return;

            // instanceID:      260:123456;

            if (!_InstanceStatistics.TryGetValue(instanceID, out instance_statistics stat))
                stat = AddNewInstanceStatisticsEntry(instanceID);

            stat.lockouts_InstanceID = lockout.InstanceID;

            stat.Dirty = true;
            Database.SaveObject(stat);
            Database.ForceSave();
        }

        public static void SavePlayerIDs(string instanceID, List<Player> plrs)
        {
            if (string.IsNullOrEmpty(instanceID) || plrs == null || plrs.Count == 0)
                return;

            // instanceID:      260:123456;
            // playerIDs:       123;456;

            if (!_InstanceStatistics.TryGetValue(instanceID, out instance_statistics stat))
                stat = AddNewInstanceStatisticsEntry(instanceID);

            string newStr = string.Empty;
            foreach (Player plr in plrs)
            {
                newStr += plr.CharacterId.ToString() + ":" + plr.Name + ";";
            }
            stat.playerIDs = newStr;

            stat.Dirty = true;
            Database.SaveObject(stat);
            Database.ForceSave();
        }

        public static void SaveTtkPerBoss(string instanceID, InstanceBossSpawn boss, TimeSpan time)
        {
            if (boss == null || time == null)
                return;

            // instanceID:      260:123456;
            // ttkPerBoss:      330:123;331:456;

            if (!_InstanceStatistics.TryGetValue(instanceID, out instance_statistics stat))
                stat = AddNewInstanceStatisticsEntry(instanceID);

            string[] split = stat.ttkPerBoss.Split(';');
            int idx = -1;
            foreach (var s in split)
            {
                if (s.Split(':')[0].Equals(boss.BossId.ToString()))
                {
                    idx = split.ToList().IndexOf(s);
                    break;
                }
            }

            if (idx == -1) // nothing found
            {
                stat.ttkPerBoss += boss.BossId + ":" + Math.Round(time.TotalSeconds, 0) + ";";
            }
            else
            {
                string[] spl = split[idx].Split(':');
                try
                {
                    string newStr = boss.BossId + ":" + Math.Round(time.TotalSeconds, 0);
                    stat.ttkPerBoss = stat.ttkPerBoss.Replace(split[idx], newStr);
                }
                catch (Exception e)
                {
                    Log.Error(e.GetType().ToString(), e.Message + "\r\n" + e.StackTrace);
                    return;
                }
            }

            stat.Dirty = true;
            Database.SaveObject(stat);
            Database.ForceSave();
        }

        public static void SaveDeathCountPerBoss(string instanceID, InstanceBossSpawn boss, int deaths)
        {
            if (boss == null)
                return;

            // instanceID:      260:123456;
            // deathCountPerBoss: 330:2;331:1;

            if (!_InstanceStatistics.TryGetValue(instanceID, out instance_statistics stat))
                stat = AddNewInstanceStatisticsEntry(instanceID);

            string[] split = stat.deathCountPerBoss.Split(';');
            int idx = -1;
            foreach (var s in split)
            {
                if (s.Split(':')[0].Equals(boss.BossId.ToString()))
                {
                    idx = split.ToList().IndexOf(s);
                    break;
                }
            }

            if (idx == -1) // nothing found
            {
                stat.deathCountPerBoss += boss.BossId + ":" + deaths + ";";
            }
            else
            {
                string[] spl = split[idx].Split(':');
                try
                {
                    string newStr = boss.BossId + ":" + (int.Parse(spl[1]) + deaths).ToString();
                    stat.deathCountPerBoss = stat.deathCountPerBoss.Replace(split[idx], newStr);
                }
                catch (Exception e)
                {
                    Log.Error(e.GetType().ToString(), e.Message + "\r\n" + e.StackTrace);
                    return;
                }
            }

            stat.Dirty = true;
            Database.SaveObject(stat);
            Database.ForceSave();
        }

        public static void SaveAttemptsPerBoss(string instanceID, InstanceBossSpawn boss, int attempts)
        {
            if (boss == null)
                return;

            // instanceID:      260:123456;
            // attemptsPerBoss: 330:2;331:1;

            if (!_InstanceStatistics.TryGetValue(instanceID, out instance_statistics stat))
                stat = AddNewInstanceStatisticsEntry(instanceID);

            string[] split = stat.attemptsPerBoss.Split(';');
            int idx = -1;
            foreach (var s in split)
            {
                if (s.Split(':')[0].Equals(boss.BossId.ToString()))
                {
                    idx = split.ToList().IndexOf(s);
                    break;
                }
            }

            if (idx == -1) // nothing found
            {
                stat.attemptsPerBoss += boss.BossId + ":" + attempts + ";";
            }
            else
            {
                string[] spl = split[idx].Split(':');
                try
                {
                    string newStr = boss.BossId + ":" + (int.Parse(spl[1]) + attempts).ToString();
                    stat.attemptsPerBoss = stat.attemptsPerBoss.Replace(split[idx], newStr);
                }
                catch (Exception e)
                {
                    Log.Error(e.GetType().ToString(), e.Message + "\r\n" + e.StackTrace);
                    return;
                }
            }

            stat.Dirty = true;
            Database.SaveObject(stat);
            Database.ForceSave();
        }

        public static instance_encounters GetInstanceEncounter(uint instanceID, uint bossId)
        {
            _InstanceEncounter.TryGetValue(instanceID, out List<instance_encounters> bosses);
            foreach (instance_encounters IE in bosses)
            {
                if (bossId == IE.bossId)
                    return IE;
            }
            return null;
        }

        public static void ClearLockouts(Player plr)
        {
            if (plr._Value.GetAllLockouts().Count == 0 || plr.Zone == null)
                return;

            _InstanceInfo.TryGetValue(plr.Zone.ZoneId, out instance_infos Info);

            if (Info == null)
                return;

            plr._Value.ClearLockouts((int)Info.LockoutTimer);
            Database.SaveObject(plr._Value);
        }

        #endregion access methods
    }
}