using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "characters_toks_kills", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class characters_toks_kills : DataObject
    {
        [PrimaryKey]
        public uint CharacterId { get; set; }

        [PrimaryKey]
        public ushort NPCEntry { get; set; }

        [DataElement()]
        public uint Count { get; set; }
    }
}