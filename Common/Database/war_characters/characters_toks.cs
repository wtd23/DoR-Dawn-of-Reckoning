using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "characters_toks", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class characters_toks : DataObject
    {
        [PrimaryKey]
        public uint CharacterId { get; set; }

        [PrimaryKey]
        public ushort TokEntry { get; set; }

        [DataElement()]
        public uint Count { get; set; }
    }
}