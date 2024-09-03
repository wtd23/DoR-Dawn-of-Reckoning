using FrameWork;
using System;

namespace Common
{
    public enum Objective_Type
    {
        QUEST_UNKNOWN = 0,
        QUEST_SPEAK_TO = 1,
        QUEST_KILL_MOB = 2,
        QUEST_USE_GO = 3,
        QUEST_GET_ITEM = 4,
        QUEST_KILL_PLAYERS = 5,
        QUEST_PROTECT_UNIT = 6,
        QUEST_USE_ITEM = 7,
        QUEST_WIN_SCENARIO = 8,
        QUEST_CAPTURE_BO = 9,
        QUEST_CAPTURE_KEEP = 10,
        QUEST_KILL_GO = 11
    };

    [DataTable(PreCache = false, TableName = "quests_objectives", DatabaseName = "World", BindMethod = EBindingMethod.StaticBound)]
    [Serializable]
    public class quests_objectives : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int Guid { get; set; }

        [DataElement()]
        public ushort Entry { get; set; }

        [DataElement()]
        public uint ObjType { get; set; }

        [DataElement()]
        public uint ObjCount { get; set; }

        [DataElement()]
        public string Description { get; set; }

        [DataElement()]
        public string ObjID { get; set; }

        [DataElement()]
        public ushort PQArea { get; set; }

        [DataElement()]
        public string inZones { get; set; }

        [DataElement()]
        public int PreviousObj { get; set; }

        public quests Quest;
        public item_infos Item = null;
        public Creature_proto Creature = null;
        public GameObject_proto GameObject = null;
        public scenario_infos Scenario = null;
        public battlefront_objectives BattleFrontObjective = null;
        public keep_infos Keep = null;
    }
}