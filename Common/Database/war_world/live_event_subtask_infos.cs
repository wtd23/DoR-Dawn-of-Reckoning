using FrameWork;
using System;

namespace Common.Database.World.LiveEvents
{
    [DataTable(PreCache = false, TableName = "live_event_subtask_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class live_event_subtask_infos : DataObject
    {
        [PrimaryKey]
        public uint Entry { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint LiveEventTaskId { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public string Description { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint TaskCount { get; set; }
    }
}