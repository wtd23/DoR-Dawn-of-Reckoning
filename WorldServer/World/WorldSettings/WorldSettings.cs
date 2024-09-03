using Common;
using WorldServer.Managers;

namespace WorldServer.World.WorldSettings
{
    public class WorldSettings
    {
        private world_settings Settings = WorldMgr.Database.SelectObject<world_settings>("SettingId = 2");
        //public bool FirstConnect =
    }
}