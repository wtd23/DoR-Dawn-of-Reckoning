using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "pquest_loot_crafting", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class pquest_loot_crafting : DataObject
    {
        [PrimaryKey]
        public byte PQCraftingBag_ID { get; set; }

        [PrimaryKey]
        public uint ItemID { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Count { get; set; }
    }
}