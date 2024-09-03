using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class QuestService : ServiceBase
    {
        public static Dictionary<ushort, quests> _Quests;

        [LoadingFunction(true)]
        public static void LoadQuests()
        {
            _Quests = Database.MapAllObjects<ushort, quests>("Entry", 5000);

            Log.Success("LoadQuests", "Loaded " + _Quests.Count + " Quests");
        }

        public static quests GetQuest(ushort QuestID)
        {
            quests Q;
            _Quests.TryGetValue(QuestID, out Q);
            return Q;
        }

        public static Dictionary<int, quests_objectives> _Objectives;

        [LoadingFunction(true)]
        public static void LoadQuestsObjectives()
        {
            _Objectives = Database.MapAllObjects<int, quests_objectives>("Guid");

            Log.Success("LoadQuestsObjectives", "Loaded " + _Objectives.Count + " Quests Objectives");
        }

        public static List<quests_maps> _QuestMaps;

        [LoadingFunction(true)]
        public static void LoadQuestsMaps()
        {
            _QuestMaps = Database.SelectAllObjects<quests_maps>() as List<quests_maps>;

            Log.Success("LoadQuestsMaps", "Loaded " + _QuestMaps.Count + " Quests Maps");
        }

        public static quests_objectives GetQuestObjective(int Guid)
        {
            quests_objectives Obj;
            _Objectives.TryGetValue(Guid, out Obj);
            return Obj;
        }

        public static Dictionary<uint, List<quests>> _CreatureStarter;

        public static void LoadQuestCreatureStarter()
        {
            _CreatureStarter = new Dictionary<uint, List<quests>>();

            IList<quests_creature_starter> Starters = Database.SelectAllObjects<quests_creature_starter>();

            if (Starters != null)
            {
                quests Q;
                foreach (quests_creature_starter Start in Starters)
                {
                    if (!_CreatureStarter.ContainsKey(Start.CreatureID))
                        _CreatureStarter.Add(Start.CreatureID, new List<quests>());

                    Q = GetQuest(Start.Entry);

                    if (Q != null)
                        _CreatureStarter[Start.CreatureID].Add(Q);
                }
            }

            Log.Success("LoadCreatureQuests", "Loaded " + _CreatureStarter.Count + " Quests Creature Starter");
        }

        public static List<quests> GetStartQuests(uint CreatureID)
        {
            List<quests> Quests;
            _CreatureStarter.TryGetValue(CreatureID, out Quests);
            return Quests;
        }

        public static Dictionary<uint, List<quests>> _CreatureFinisher;

        public static void LoadQuestCreatureFinisher()
        {
            _CreatureFinisher = new Dictionary<uint, List<quests>>();

            IList<quests_creature_finisher> Finishers = Database.SelectAllObjects<quests_creature_finisher>();

            if (Finishers != null)
            {
                quests Q;
                foreach (quests_creature_finisher Finisher in Finishers)
                {
                    if (!_CreatureFinisher.ContainsKey(Finisher.CreatureID))
                        _CreatureFinisher.Add(Finisher.CreatureID, new List<quests>());

                    Q = GetQuest(Finisher.Entry);

                    if (Q != null)
                        _CreatureFinisher[Finisher.CreatureID].Add(Q);
                }
            }

            Log.Success("LoadCreatureQuests", "Loaded " + _CreatureFinisher.Count + " Quests Creature Finisher");
        }

        public static List<quests> GetFinishersQuests(uint CreatureID)
        {
            List<quests> Quests;
            _CreatureFinisher.TryGetValue(CreatureID, out Quests);
            return Quests;
        }

        public static uint GetQuestCreatureFinisher(ushort QuestId)
        {
            foreach (KeyValuePair<uint, List<quests>> Kp in _CreatureFinisher)
            {
                foreach (quests Q in Kp.Value)
                    if (Q.Entry == QuestId)
                        return Kp.Key;
            }

            return 0;
        }

        public static bool HasQuestToFinish(uint CreatureID, ushort QuestID)
        {
            List<quests> Quests;
            if (_CreatureFinisher.TryGetValue(CreatureID, out Quests))
            {
                foreach (quests Q in Quests)
                    if (Q.Entry == QuestID)
                        return true;
            }

            return false;
        }
    }
}