using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "scenarios_durations", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class scenarios_durations : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int Guid { get; set; }

        [DataElement]
        public ushort ScenarioId { get; set; }

        [DataElement]
        public byte Tier { get; set; }

        [DataElement]
        public long StartTime { get; set; }

        [DataElement]
        public uint DurationSeconds { get; set; }
    }
}