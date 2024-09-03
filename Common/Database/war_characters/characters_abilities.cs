using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "characters_abilities", DatabaseName = "Characters", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class characters_abilities : DataObject
    {
        [DataElement()]
        public int CharacterID { get; set; }

        [DataElement()]
        public ushort AbilityID { get; set; }

        [DataElement()]
        public int LastCast { get; set; }
    }
}