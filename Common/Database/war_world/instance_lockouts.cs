using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "instance_lockouts", DatabaseName = "World")]
    [Serializable]
    public class instance_lockouts : DataObject
    {
        [DataElement]
        public string InstanceID { get; set; }

        [DataElement]
        public string Bosseskilled { get; set; }
    }
}