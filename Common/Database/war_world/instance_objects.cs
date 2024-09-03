using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_objects", DatabaseName = "World")]
    [Serializable]
    public class instance_objects : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public uint InstanceID { get; set; }

        [DataElement]
        public uint EncounterID { get; set; }

        [DataElement]
        public uint DoorID { get; set; }

        [DataElement]
        public uint GameObjectSpawnID { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public uint WorldX { get; set; }

        [DataElement]
        public uint WorldY { get; set; }

        [DataElement]
        public uint WorldZ { get; set; }

        [DataElement]
        public uint WorldO { get; set; }

        [DataElement]
        public uint DisplayID { get; set; }

        [DataElement]
        public uint VfxState { get; set; }

        public List<instance_spawn_states> Scripts = new List<instance_spawn_states>();
        public List<instance_attributes> Attributes = new List<instance_attributes>();
        public List<instance_events> Events = new List<instance_events>();

        public override string ToString()
        {
            return Name;
        }
    }
}