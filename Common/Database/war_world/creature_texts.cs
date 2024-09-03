using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "creature_texts", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class creature_texts : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int Guid { get; set; }

        [DataElement()]
        public uint Entry { get; set; }

        [DataElement()]
        public string Text { get; set; }
    }
}