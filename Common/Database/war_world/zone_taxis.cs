using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "zone_taxis", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class zone_taxis : DataObject
    {
        [PrimaryKey]
        public ushort ZoneID { get; set; }

        [PrimaryKey]
        public byte RealmID { get; set; }

        [DataElement()]
        public uint WorldX { get; set; }

        [DataElement()]
        public uint WorldY { get; set; }

        [DataElement()]
        public ushort WorldZ { get; set; }

        [DataElement()]
        public ushort WorldO { get; set; }

        [DataElement()]
        public byte Tier { get; set; }

        [DataElement()]
        public bool Enable { get; set; }

        public zone_infos Info;
    }
}