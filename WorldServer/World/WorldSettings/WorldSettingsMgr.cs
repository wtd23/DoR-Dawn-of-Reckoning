﻿using Common;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Managers;

namespace WorldServer.World.WorldSettings
{
    public class WorldSettingsMgr
    {
        // Medallions setting
        //World_Settings WorldSettings = WorldMgr.Database.SelectObject<World_Settings>("SettingId = 2");
        private List<World_Settings> WorldSettings = WorldMgr.Database.SelectAllObjects<World_Settings>().ToList();

        public WorldSettingsMgr()
        {
        }

        public int GetMedallionsSetting()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 2)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;

            //int setting = WorldSettings.Setting;
            //return setting;
        }

        public void SetMedallionsSetting(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 2)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
            //WorldSettings.Setting = newSetting;
            //WorldMgr.Database.SaveObject(WorldSettings);
            //WorldMgr.Database.ForceSave();
        }

        public int GetPopRewardSwitchSetting()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 3)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetPopRewardSwitchSetting(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 3)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }

        /// <summary>
        /// This is used to scale Supplies generated by Proximity Flag to reinforce Keep
        /// </summary>
        /// <returns></returns>
        public int GetSuppliesScaler()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 4)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetSuppliesScaler(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 4)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }

        /// <summary>
        /// This is how much % one door regenerates per 1 controled BattlefieldObjective
        /// </summary>
        /// <returns></returns>
        public int GetDoorRegenValue()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 5)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetDoorRegenValue(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 5)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }

        public int GetMovementPacketThrotle()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 6)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetMovementPacketThrotle(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 6)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }

        public int GetAmmoRefresh()
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 7)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetAmmoRefresh(int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == 7)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }

        public int GetGenericSetting(int settingId)
        {
            int setting = 0;

            foreach (var set in WorldSettings)
            {
                if (set.SettingId == settingId)
                {
                    setting = set.Setting;
                    break;
                }
            }
            return setting;
        }

        public void SetGenericSetting(int settingId, int newSetting)
        {
            foreach (var set in WorldSettings)
            {
                if (set.SettingId == settingId)
                {
                    set.Setting = newSetting;
                    WorldMgr.Database.SaveObject(set);
                    WorldMgr.Database.ForceSave();
                    break;
                }
            }
        }
    }
}