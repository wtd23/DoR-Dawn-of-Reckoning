using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "rally_points", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class rally_points : DataObject
    {
        [PrimaryKey]
        public ushort Id { get; set; }

        [DataElement()]
        public uint CreatureId { get; set; }

        [DataElement()]
        public string Name { get; set; }

        [DataElement()]
        public ushort ZoneID { get; set; }

        [DataElement()]
        public uint WorldX { get; set; }

        [DataElement()]
        public uint WorldY { get; set; }

        [DataElement()]
        public ushort WorldZ { get; set; }

        [DataElement()]
        public ushort WorldO { get; set; }
    }
}