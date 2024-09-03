using FrameWork;
using System;

namespace Common.Database.World.Battlefront
{
    [DataTable(PreCache = false, TableName = "rvr_player_kill_reward_history", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class rvr_player_kill_reward_history : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int KillId { get; set; }

        [DataElement(AllowDbNull = false), PrimaryKey]
        public int ZoneId { get; set; }

        [DataElement(AllowDbNull = false)]
        public DateTime Timestamp { get; set; }

        [DataElement(AllowDbNull = false)]
        public int KillerCharacterId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int VictimCharacterId { get; set; }

        [DataElement(AllowDbNull = false)]
        public int ItemId { get; set; }

        [DataElement(AllowDbNull = false)]
        public string ItemName { get; set; }

        [DataElement(AllowDbNull = false)]
        public string KillerCharacterName { get; set; }

        [DataElement(AllowDbNull = false)]
        public string VictimCharacterName { get; set; }

        [DataElement(AllowDbNull = false)]
        public string ZoneName { get; set; }
    }
}