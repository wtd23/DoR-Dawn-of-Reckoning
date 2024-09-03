using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class XpRenownService : ServiceBase
    {
        private static Dictionary<byte, xp_infos> _xpInfos;

        [LoadingFunction(true)]
        public static void LoadXp_Info()
        {
            Log.Debug("WorldMgr", "Loading Xp_Infos...");

            _xpInfos = Database.MapAllObjects<byte, xp_infos>("Level");

            Log.Success("LoadXp_Info", "Loaded " + _xpInfos.Count + " Xp_Infos");
        }

        public static xp_infos GetXp_Info(byte Level)
        {
            xp_infos info;
            _xpInfos.TryGetValue(Level, out info);
            return info;
        }

        private static Dictionary<byte, renown_infos> _renownInfos;

        [LoadingFunction(true)]
        public static void LoadRenown_Info()
        {
            Log.Debug("WorldMgr", "Loading Renown_Info...");

            _renownInfos = new Dictionary<byte, renown_infos>();
            foreach (renown_infos Info in Database.SelectAllObjects<renown_infos>())
                _renownInfos.Add(Info.Level, Info);

            Log.Success("LoadRenown_Info", "Loaded " + _renownInfos.Count + " Renown_Info");
        }

        public static renown_infos GetRenown_Info(byte level)
        {
            renown_infos info;
            _renownInfos.TryGetValue(level, out info);
            return info;
        }
    }
}