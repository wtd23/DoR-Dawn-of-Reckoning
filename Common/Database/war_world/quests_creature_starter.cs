using FrameWork;
using System;

namespace Common
{
    [DataTable(PreCache = false, TableName = "quests_creature_starter", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class quests_creature_starter : DataObject
    {
        [PrimaryKey]
        public ushort Entry { get; set; }

        [DataElement()]
        public uint CreatureID { get; set; }
    }
}