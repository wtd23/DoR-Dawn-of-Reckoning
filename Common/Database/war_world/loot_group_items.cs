using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "loot_group_items", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class loot_group_items : DataObject
    {
        [DataElement()]
        public UInt32 LootGroupID { get; set; }

        [DataElement()]
        public UInt32 ItemID { get; set; }

        [DataElement()]
        public byte MinRank { get; set; }

        [DataElement()]
        public byte MaxRank { get; set; }

        [DataElement()]
        public byte MinRenown { get; set; }

        [DataElement()]
        public byte MaxRenown { get; set; }

        public loot_groups Loot_Group;
        public item_infos Item;
    }
}