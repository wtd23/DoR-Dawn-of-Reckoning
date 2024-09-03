using System.Collections.Generic;

namespace Common.Database.World.Maps
{
    /// <summary>
    /// Entity computed on server startup providing spawning elements in a zone cell.
    /// </summary>
    public class cell_spawns
    {
        private ushort _x, _y, _regionId;
        public List<creature_spawns> CreatureSpawns = new List<creature_spawns>();
        public List<gameobject_spawns> GameObjectSpawns = new List<gameobject_spawns>();
        public List<chapter_infos> ChapterSpawns = new List<chapter_infos>();
        public List<pquest_info> PublicQuests = new List<pquest_info>();

        public cell_spawns(ushort regionId, ushort x, ushort y)
        {
            _regionId = regionId;
            _x = x;
            _y = y;
        }

        public void AddSpawn(creature_spawns spawn)
        {
            CreatureSpawns.Add(spawn);
        }

        public void AddSpawn(gameobject_spawns spawn)
        {
            GameObjectSpawns.Add(spawn);
        }

        public void AddChapter(chapter_infos chapter)
        {
            chapter.OffX = _x;
            chapter.OffY = _y;

            ChapterSpawns.Add(chapter);
        }

        public void AddPQuest(pquest_info pQuest)
        {
            pQuest.OffX = _x;
            pQuest.OffY = _y;

            PublicQuests.Add(pQuest);
        }
    }
}