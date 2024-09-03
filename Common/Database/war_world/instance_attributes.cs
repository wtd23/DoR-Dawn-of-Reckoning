using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_attributes", DatabaseName = "World")]
    [Serializable]
    public class instance_attributes : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public ushort Type { get; set; }

        [DataElement]
        public string Value { get; set; }

        [DataElement]
        public ushort AttachToTableID { get; set; }

        [DataElement]
        public uint AttachToID { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}