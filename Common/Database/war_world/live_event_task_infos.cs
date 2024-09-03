using FrameWork;
using System;
using System.Collections.Generic;

namespace Common.Database.World.LiveEvents
{
    [DataTable(PreCache = false, TableName = "live_event_task_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class live_event_task_infos : DataObject
    {
        [PrimaryKey]
        public uint Entry { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint LiveEventId { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public string Description { get; set; }

        [DataElement(AllowDbNull = false)]
        public int TotalTasks { get; set; }

        public List<live_event_subtask_infos> Tasks { get; set; } = new List<live_event_subtask_infos>();
    }
}