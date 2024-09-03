using FrameWork;
using System;

namespace Common.Database.World.LiveEvents
{
    [DataTable(PreCache = false, TableName = "live_event_reward_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class live_event_reward_infos : DataObject
    {
        [PrimaryKey]
        public uint Entry { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint LiveEventId { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint RewardGroupId { get; set; } //1 to 3

        [DataElement(AllowDbNull = false)]
        public uint ItemId { get; set; }

        public item_infos Item { get; set; }
    }
}