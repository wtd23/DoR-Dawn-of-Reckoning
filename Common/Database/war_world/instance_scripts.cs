using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_scripts", DatabaseName = "World")]
    [Serializable]
    public class instance_scripts : DataObject
    {
        [DataElement]
        public uint Entry { get; set; }

        [DataElement]
        public string Name { get; set; }

        [DataElement]
        public uint InstanceID { get; set; }

        [DataElement]
        public string Script { get; set; }

        public object Object;

        public override string ToString()
        {
            return Name;
        }
    }
}