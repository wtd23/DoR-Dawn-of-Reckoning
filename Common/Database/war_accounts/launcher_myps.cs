using FrameWork;
using System;

namespace Common.Database.Account
{
    [DataTable(PreCache = false, TableName = "launcher_myps", DatabaseName = "Accounts", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class launcher_myps : DataObject
    {
        private int _Id;
        private string _name;
        private int _CRC32;

        [PrimaryKey(AutoIncrement = true)]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; Dirty = true; }
        }

        [DataElement(Varchar = 2000)]
        public string Name
        {
            get { return _name; }
            set { _name = value; Dirty = true; }
        }

        [DataElement]
        public int CRC32
        {
            get { return _CRC32; }
            set { _CRC32 = value; Dirty = true; }
        }

        public ulong ExtractedSize { get; set; }
        public uint AssetCount { get; set; }
    }
}