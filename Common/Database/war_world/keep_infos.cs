using Common.Database.World.Battlefront;
using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "keep_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class keep_infos : DataObject
    {
        private byte _KeepId;
        private string _Name;
        private byte _Realm;
        private byte _Race;
        private byte _DoorCount;
        private ushort _ZoneId;
        private ushort _RegionId;
        private ushort _PQuestId;

        [PrimaryKey]
        public byte KeepId
        {
            get { return _KeepId; }
            set { _KeepId = value; Dirty = true; }
        }

        [DataElement(Varchar = 255)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Realm
        {
            get { return _Realm; }
            set { _Realm = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Race
        {
            get { return _Race; }
            set { _Race = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte DoorCount
        {
            get { return _DoorCount; }
            set { _DoorCount = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort RegionId
        {
            get { return _RegionId; }
            set { _RegionId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort PQuestId
        {
            get { return _PQuestId; }
            set { _PQuestId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int X { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Y { get; set; }

        [DataElement(AllowDbNull = false)]
        public int Z { get; set; }

        [DataElement(AllowDbNull = false)]
        public int O { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool IsFortress { get; set; }

        [DataElement(AllowDbNull = false)]
        public int GuildClaimObjectiveId { get; set; }

        public List<keep_spawn_points> KeepSiegeSpawnPoints { get; set; }
        public List<keep_creatures> Creatures;
        public List<keep_doors> Doors;
        public pquest_info PQuest;
    }
}