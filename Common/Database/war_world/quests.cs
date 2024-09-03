using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "quests", DatabaseName = "World")]
    [Serializable]
    public class quests : DataObject
    {
        [PrimaryKey]
        public ushort Entry { get; set; }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Name { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte Type { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MinLevel { get; set; }

        [DataElement(AllowDbNull = false)]
        public string Description { get; set; }

        [DataElement(AllowDbNull = false)]
        public string OnCompletionQuest { get; set; }

        [DataElement(AllowDbNull = false)]
        public string ProgressText { get; set; }

        [DataElement(AllowDbNull = false)]
        public string Particular { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint Xp { get; set; }

        [DataElement(AllowDbNull = false)]
        public uint Gold { get; set; }

        [DataElement(AllowDbNull = false)]
        public string Given { get; set; }

        [DataElement(AllowDbNull = false)]
        public string Choice { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte ChoiceCount { get; set; }

        [DataElement(AllowDbNull = false)]
        public ushort PrevQuest { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool Repeatable { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MaxLevel { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MinRenown { get; set; }

        [DataElement(AllowDbNull = false)]
        public byte MaxRenown { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool Active { get; set; }

        [DataElement(AllowDbNull = false)]
        public bool Shareable { get; set; }

        public List<quests_objectives> Objectives = new List<quests_objectives>();
        public List<quests_maps> Maps = new List<quests_maps>();
        public Dictionary<item_infos, uint> Rewards = new Dictionary<item_infos, uint>();
    }
}