using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "guild_xp", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class guild_xp : DataObject
    {
        public byte _Level;
        public uint _Xp;

        [PrimaryKey]
        public byte Level
        {
            get { return _Level; }
            set { _Level = value; }
        }

        [DataElement()]
        public uint Xp
        {
            get { return _Xp; }
            set { _Xp = value; }
        }
    }
}