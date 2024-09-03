using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_infos", DatabaseName = "World")]
    [Serializable]
    public class instance_infos : DataObject
    {
        [DataElement]
        public ushort Entry { get; set; }

        [DataElement]
        public ushort ZoneID { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public uint LockoutTimer { get; set; }

        [DataElement]
        public uint TrashRespawnTimer { get; set; }

        [DataElement]
        public byte WardsNeeded { get; set; }

        [DataElement]
        public uint OrderExitZoneJumpID { get; set; }

        [DataElement]
        public uint DestrExitZoneJumpID { get; set; }

        public List<instance_scripts> Scripts = new List<instance_scripts>();
        public List<instance_attributes> Attributes = new List<instance_attributes>();
        public List<instance_encounters> Encounters = new List<instance_encounters>();
        public List<instance_creature_spawns> Monsters = new List<instance_creature_spawns>();
        public List<instance_objects> Objects = new List<instance_objects>();
        public List<instance_events> Events = new List<instance_events>();

        public override string ToString()
        {
            return Name;
        }
    }
}