using Common.Database.World.Characters;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class HonorService : ServiceBase
    {
        public static List<honor_rewards> HonorRewards;

        /// <summary>
        /// List of Honor Rewards
        /// </summary>
        [LoadingFunction(true)]
        public static void LoadHonorRewards()
        {
            Log.Debug("WorldMgr", "Loading Honor Rewards...");
            HonorRewards = Database.SelectAllObjects<honor_rewards>() as List<honor_rewards>;
            if (HonorRewards != null) Log.Success("HonorRewards", "Loaded " + HonorRewards.Count + " HonorRewards");
        }
    }
}