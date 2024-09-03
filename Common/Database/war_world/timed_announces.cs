using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "timed_announces", DatabaseName = "World")]
    [Serializable]
    public class timed_announces : DataObject
    {
        [DataElement()]
        public string SenderName { get; set; }

        [DataElement()]
        public string Message { get; set; }

        [DataElement()]
        public ushort ZoneId { get; set; }

        [DataElement()]
        public byte Realm { get; set; }

        [DataElement()]
        public byte Type { get; set; }

        [DataElement()]
        public int NextTime { get; set; }
    }
}