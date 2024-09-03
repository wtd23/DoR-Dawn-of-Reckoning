using Common.Database.World.Battlefront;
using FrameWork;
using System.Collections.Generic;
using System.Linq;
using WorldServer.World.Battlefronts.Apocalypse;

namespace WorldServer.Services.World
{
    [Service]
    public class RVRProgressionService : ServiceBase
    {
        public static List<rvr_progression> RVRProgressions;
        public static List<pairing_infos> RVRPairings;
        public static List<campaign_objective_buff> CampaignObjectiveBuffs;
        public static List<rvr_area_polygon> RVRAreaPolygons;

        [LoadingFunction(true)]
        public static void LoadRVRProgressions()
        {
            Log.Debug("WorldMgr", "Loading RVR Progression...");
            RVRProgressions = Database.SelectAllObjects<rvr_progression>() as List<rvr_progression>;
            Log.Success("RVRProgression", "Loaded " + RVRProgressions.Count + " RVRProgressions");
        }

        [LoadingFunction(true)]
        public static void LoadPairings()
        {
            Log.Debug("WorldMgr", "Loading RVR Pairings...");
            RVRPairings = Database.SelectObjects<pairing_infos>("Enabled=1") as List<pairing_infos>;
            Log.Success("RVRProgression", "Loaded " + RVRProgressions.Count + " Pairings");
        }

        [LoadingFunction(true)]
        public static void LoadCampaignObjectiveBuffs()
        {
            Log.Debug("WorldMgr", "Loading Campaign Objective Buffs...");
            CampaignObjectiveBuffs = Database.SelectAllObjects<campaign_objective_buff>() as List<campaign_objective_buff>;
            Log.Success("RVRProgression", "Loaded " + CampaignObjectiveBuffs.Count + " Campaign Objective Buffs");
        }

        [LoadingFunction(true)]
        public static void LoadRVRAreaPolygons()
        {
            Log.Debug("RVRProgression", "Loading RVR Area Polygons...");
            RVRAreaPolygons = Database.SelectAllObjects<rvr_area_polygon>() as List<rvr_area_polygon>;
            Log.Success("RVRProgression", "Loaded " + RVRAreaPolygons.Count + " RVR Area Polygons");
        }

        public static void SaveRVRProgression(List<rvr_progression> rvrProg)
        {
            if (rvrProg == null || rvrProg.Count <= 0)
                return;

            Log.Debug("WorldMgr", "Saving RVR progression ...");

            foreach (var item in rvrProg)
            {
                item.Dirty = true;
                item.IsValid = true;
                Database.SaveObject(item);
                item.Dirty = false;
            }

            Database.ForceSave();

            Log.Dump("RVRProgression", $"Saved RVR progression in tier {rvrProg.FirstOrDefault().Tier}");
        }

        public static void SaveBattleFrontKeepState(byte keepId, SM.ProcessState state)
        {
            var statusEntity = new battlefront_keep_status { KeepId = keepId, Status = (int)state };

            Log.Debug("WorldMgr", $"Saving battlefront keep status {keepId} {(int)state}...");
            RemoveBattleFrontKeepStatus(keepId);
            Database.SaveObject(statusEntity);
            Database.ForceSave();
        }

        public static void RemoveBattleFrontKeepStatus(byte keepId)
        {
            Database.ExecuteNonQuery($"DELETE FROM battlefront_keep_status WHERE keepId={keepId}");
        }

        public static battlefront_keep_status GetBattleFrontKeepStatus(byte keepId)
        {
            var status = Database.SelectObject<battlefront_keep_status>($"KeepId={keepId}");
            return status;
        }
    }
}