using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class ChapterService : ServiceBase
    {
        public static Dictionary<uint, chapter_infos> _Chapters;

        [LoadingFunction(true)]
        public static void LoadChapter_Infos()
        {
            Log.Debug("WorldMgr", "Loading Chapter_Infos...");

            _Chapters = Database.MapAllObjects<uint, chapter_infos>("Entry");

            Log.Success("LoadChapter_Infos", "Loaded " + _Chapters.Count + " Chapter_Infos");
        }

        public static chapter_infos GetChapter(uint Entry)
        {
            chapter_infos Info;
            _Chapters.TryGetValue(Entry, out Info);
            return Info;
        }

        public static ushort GetChapterByNPCID(uint Entry)
        {
            foreach (chapter_infos chapter in _Chapters.Values)
                if (chapter.CreatureEntry == Entry)
                    return (ushort)chapter.InfluenceEntry;
            return 0;
        }

        // Function is unused
        public static List<chapter_infos> GetChapters(ushort ZoneId)
        {
            List<chapter_infos> Chapters = new List<chapter_infos>();

            foreach (chapter_infos chapter in _Chapters.Values)
                if (chapter.ZoneId == ZoneId)
                    Chapters.Add(chapter);

            return Chapters;
        }

        public static chapter_infos GetChapterEntry(ushort InfluenceEntry)
        {
            List<chapter_infos> Chapters = new List<chapter_infos>();

            foreach (chapter_infos chapter in _Chapters.Values)
                if (chapter.InfluenceEntry == InfluenceEntry)
                    return chapter;
            return null;
        }

        public static Dictionary<uint, List<chapter_rewards>> _Chapters_Reward;

        [LoadingFunction(true)]
        public static void LoadChapter_Rewards()
        {
            Log.Debug("WorldMgr", "Loading LoadChapter_Rewards...");

            _Chapters_Reward = new Dictionary<uint, List<chapter_rewards>>();
            IList<chapter_rewards> Rewards = Database.SelectAllObjects<chapter_rewards>();

            foreach (chapter_rewards Reward in Rewards)
            {
                if (!_Chapters_Reward.ContainsKey(Reward.Entry))
                    _Chapters_Reward.Add(Reward.Entry, new List<chapter_rewards>());

                _Chapters_Reward[Reward.Entry].Add(Reward);
            }

            Log.Success("LoadChapter_Infos", "Loaded " + Rewards.Count + " Chapter_Rewards");
        }

        public static List<chapter_rewards> GetChapterRewards(uint Entry)
        {
            List<chapter_rewards> Info;
            _Chapters_Reward.TryGetValue(Entry, out Info);
            return Info;
        }
    }
}