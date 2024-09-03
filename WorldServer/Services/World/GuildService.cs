using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class GuildService : ServiceBase
    {
        private static Dictionary<byte, guild_xp> _Guild_Xps;

        [LoadingFunction(true)]
        public static void LoadGuild_Info()
        {
            Log.Debug("WorldMgr", "Loading Guild_Xps...");

            _Guild_Xps = Database.MapAllObjects<byte, guild_xp>("Level", 20);

            Log.Success("LoadGuild_Info", "Loaded " + _Guild_Xps.Count + " Guild_Xps");
        }

        public static guild_xp GetGuild_Xp(byte Level)
        {
            if (_Guild_Xps.ContainsKey(Level))
                return _Guild_Xps[Level];
            return null;
        }
    }
}