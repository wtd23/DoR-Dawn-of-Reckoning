using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "gm_commands_logs", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class gm_commands_logs : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int Guid { get; set; }

        [DataElement()]
        public uint AccountId { get; set; }

        [DataElement(Varchar = 255)]
        public string PlayerName { get; set; }

        [DataElement()]
        public string Command { get; set; }

        [DataElement()]
        public DateTime Date { get; set; }
    }
}