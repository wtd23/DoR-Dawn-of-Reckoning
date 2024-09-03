using Common.Database.World.Battlefront;
using FrameWork;
using System.Collections.Generic;
using WorldServer.World.Objects;

namespace WorldServer.Services.World
{
    [Service]
    public class RVRZoneRewardService : ServiceBase
    {
        private static List<rvr_reward_keep_items> _RVRRewardKeepItems;
        private static List<rvr_reward_fort_items> _RVRRewardFortItems;

        public static List<RVRRewardItem> RVRRewardKeepItems;
        public static List<RVRRewardItem> RVRRewardFortItems;
        public static List<rvr_zone_lock_reward> RVRZoneLockRewards;
        public static List<rvr_keep_lock_reward> RVRKeepLockRewards;

        /// <summary>
        /// List of RVR Zone Lock items that are to be considered on a zone lock
        /// </summary>
        [LoadingFunction(true)]
        public static void LoadRVRZoneLockItemOptions()
        {
            Log.Debug("WorldMgr", "Loading RVR Zone Lock Item Options...");
            _RVRRewardKeepItems = Database.SelectAllObjects<rvr_reward_keep_items>() as List<rvr_reward_keep_items>;
            if (_RVRRewardKeepItems != null) Log.Success("LoadRVRZoneLockItemOptions", "Loaded " + _RVRRewardKeepItems.Count + " RVR Zone Lock Item Options");

            if (RVRRewardKeepItems == null)
                RVRRewardKeepItems = new List<RVRRewardItem>();

            foreach (var rvrRewardKeepItem in _RVRRewardKeepItems)
            {
                RVRRewardKeepItems.Add(new RVRRewardItem(rvrRewardKeepItem));
            }
        }

        /// <summary>
        /// List of RVR Zone Lock items that are to be considered on a zone lock
        /// </summary>
        [LoadingFunction(true)]
        public static void LoadFortZoneLockOptions()
        {
            Log.Debug("WorldMgr", "Loading RVR Fort Zone Lock Options...");
            _RVRRewardFortItems = Database.SelectAllObjects<rvr_reward_fort_items>() as List<rvr_reward_fort_items>;
            if (_RVRRewardFortItems != null) Log.Success("LoadFortZoneLockOptions", "Loaded " + _RVRRewardFortItems.Count + " RVRRewardFortItems");

            if (RVRRewardFortItems == null)
                RVRRewardFortItems = new List<RVRRewardItem>();

            foreach (var rvrRewardKeepItem in _RVRRewardFortItems)
            {
                RVRRewardFortItems.Add(new RVRRewardItem(rvrRewardKeepItem));
            }
        }

        /// <summary>
        /// List of rewards, regardless of item consideration (ie crests, RR, money, etc)
        /// </summary>
        [LoadingFunction(true)]
        public static void LoadRVRZoneLockRewards()
        {
            Log.Debug("WorldMgr", "Loading RVR Zone Lock Rewards...");
            RVRZoneLockRewards = Database.SelectAllObjects<rvr_zone_lock_reward>() as List<rvr_zone_lock_reward>;
            if (RVRZoneLockRewards != null) Log.Success("RVRZoneReward", "Loaded " + RVRZoneLockRewards.Count + " RVRZoneReward");
        }

        /// <summary>
        /// List of rewards, regardless of item consideration (ie crests, RR, money, etc)
        /// </summary>
        [LoadingFunction(true)]
        public static void LoadRVRKeepLockRewards()
        {
            Log.Debug("WorldMgr", "Loading RVR Keep Lock Rewards...");
            RVRKeepLockRewards = Database.SelectAllObjects<rvr_keep_lock_reward>() as List<rvr_keep_lock_reward>;
            if (RVRKeepLockRewards != null) Log.Success("RVRKeepLockRewards", "Loaded " + RVRKeepLockRewards.Count + " RVRKeepLockRewards");
        }
    }
}