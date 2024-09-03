using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_spawn_state_abilities", DatabaseName = "World")]
    [Serializable]
    public class instance_spawn_state_abilities : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public uint InstanceSpawnStateID { get; set; }

        [DataElement]
        public uint AbilityID { get; set; }

        [DataElement]
        public string Note { get; set; }

        public List<instance_attributes> Attributes = new List<instance_attributes>();

        public override string ToString()
        {
            return Note;
        }
    }
}