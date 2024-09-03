using FrameWork;
using System;
using System.Collections.Generic;

namespace Common
{
    [DataTable(PreCache = false, TableName = "chapter_infos", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class chapter_infos : DataObject
    {
        [PrimaryKey()]
        public uint Entry { get; set; }

        [DataElement()]
        public ushort ZoneId { get; set; }

        [DataElement(Varchar = 255)]
        public string Name { get; set; }

        [DataElement()]
        public uint CreatureEntry { get; set; }

        [DataElement()]
        public uint InfluenceEntry { get; set; }

        [DataElement(Varchar = 30)]
        public string Race { get; set; }

        [DataElement()]
        public uint ChapterRank { get; set; }

        [DataElement()]
        public ushort PinX { get; set; }

        [DataElement()]
        public ushort PinY { get; set; }

        [DataElement()]
        public ushort TokEntry { get; set; }

        [DataElement()]
        public uint TokExploreEntry { get; set; }

        [DataElement()]
        public uint Tier1InfluenceCount { get; set; }

        [DataElement()]
        public uint Tier2InfluenceCount { get; set; }

        [DataElement()]
        public uint Tier3InfluenceCount { get; set; }

        public ushort OffX;
        public ushort OffY;
        public uint MaxInflu;
        public List<chapter_rewards> T1Rewards;
        public List<chapter_rewards> T2Rewards;
        public List<chapter_rewards> T3Rewards;
    }
}