using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "character_info_stats", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class character_info_stats : DataObject
    {
        private byte _CareerLine;
        private byte _Level;
        private byte _StatId;
        private ushort _StatValue;

        [PrimaryKey]
        public byte CareerLine
        {
            get { return _CareerLine; }
            set { _CareerLine = value; Dirty = true; }
        }

        [PrimaryKey]
        public byte Level
        {
            get { return _Level; }
            set { _Level = value; Dirty = true; }
        }

        [PrimaryKey]
        public byte StatId
        {
            get { return _StatId; }
            set { _StatId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort StatValue
        {
            get { return _StatValue; }
            set { _StatValue = value; Dirty = true; }
        }
    }
}