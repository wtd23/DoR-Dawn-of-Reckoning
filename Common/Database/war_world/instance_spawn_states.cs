using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_spawn_states", DatabaseName = "World")]
    [Serializable]
    public class instance_spawn_states : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public uint InstanceSpawnID { get; set; }

        [DataElement]
        public uint InstanceObjectID { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public string Note { get; set; }

        public List<instance_spawn_state_abilities> Abilities = new List<instance_spawn_state_abilities>();
        public List<instance_attributes> Attributes = new List<instance_attributes>();

        public override string ToString()
        {
            return Name;
        }
    }
}