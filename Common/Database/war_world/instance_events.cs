using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_events", DatabaseName = "World")]
    [Serializable]
    public class instance_events : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public uint InstanceID { get; set; }

        [DataElement]
        public uint EncounterID { get; set; }

        [DataElement]
        public uint InstanceSpawnID { get; set; }

        [DataElement]
        public uint InstanceObjectID { get; set; }

        [DataElement]
        public ushort EventType { get; set; }

        [DataElement]
        public string Note { get; set; }

        public List<instance_attributes> Attributes = new List<instance_attributes>();
        public List<instance_event_commands> Commands = new List<instance_event_commands>();
    }
}