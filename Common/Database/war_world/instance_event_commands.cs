using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_event_commands", DatabaseName = "World")]
    [Serializable]
    public class instance_event_commands : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public uint InstanceEventID { get; set; }

        [DataElement]
        public ushort CommandType { get; set; }

        [DataElement]
        public string Note { get; set; }

        public List<instance_attributes> Attributes = new List<instance_attributes>();
    }
}