using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "battlefront_objectives", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class battlefront_objectives : DataObject
    {
        [PrimaryKey]
        public int Entry { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort RegionId { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort ZoneId { get; set; }

        [DataElement(AllowDbNull = false)]
        public string Name { get; set; }

        [DataElement(AllowDbNull = false)]
        public int X { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Y { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Z { get; set; }

        [DataElement(AllowDbNull = false)]
        public int O { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint TokDiscovered { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint TokUnlocked { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool KeepSpawn { get; set; }

        public List<battlefront_guards> Guards;
    }
}