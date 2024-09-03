using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "chapter_rewards", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class chapter_rewards : DataObject
    {
        [PrimaryKey]
        public uint Entry { get; set; }

        [DataElement]
        public byte Realm { get; set; }

        [PrimaryKey]
        public uint ItemId { get; set; }

        [DataElement]
        public uint InfluenceCount { get; set; }

        public chapter_infos Chapter;
        public item_infos Item;
    }
}