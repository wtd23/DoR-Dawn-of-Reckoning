using Common;
using FrameWork;
using System.Collections.Generic;
using System.Threading;

namespace WorldServer.Services.World
{
    [Service(typeof(ItemService))]
    public class GameObjectService : ServiceBase
    {
        #region GameObjects

        public static Dictionary<uint, GameObject_proto> GameObjectProtos;
        public static Dictionary<uint, gameobject_spawns> GameObjectSpawns;
        public static int MaxGameObjectGUID;

        public static int GenerateGameObjectSpawnGUID()
        {
            return Interlocked.Increment(ref MaxGameObjectGUID);
        }

        [LoadingFunction(true)]
        public static void LoadGameObjectProtos()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Protos...");

            GameObjectProtos = Database.MapAllObjects<uint, GameObject_proto>("Entry");

            Log.Success("WorldMgr", "Loaded " + GameObjectProtos.Count + " GameObject_Protos");
        }

        public static GameObject_proto GetGameObjectProto(uint Entry)
        {
            GameObject_proto Proto;
            GameObjectProtos.TryGetValue(Entry, out Proto);
            return Proto;
        }

        [LoadingFunction(true)]
        public static void LoadGameObjectSpawns()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Spawns...");

            GameObjectSpawns = Database.MapAllObjects<uint, gameobject_spawns>("Guid", 25000);

            foreach (gameobject_spawns Spawn in GameObjectSpawns.Values)
            {
                if (Spawn.Guid > MaxGameObjectGUID)
                    MaxGameObjectGUID = (int)Spawn.Guid;
            }

            Log.Success("WorldMgr", "Loaded " + GameObjectSpawns.Count + " GameObject_Spawns");
        }

        #endregion GameObjects

        #region GameObjectLoots

        public static Dictionary<uint, List<gameobject_loots>> GameObjectLoots = new Dictionary<uint, List<gameobject_loots>>();

        private static void LoadGameObjectLoots(uint entry)
        {
            if (!GameObjectLoots.ContainsKey(entry))
            {
                Log.Debug("WorldMgr", "Loading GameObject Loots of " + entry + " ...");

                List<gameobject_loots> Loots = new List<gameobject_loots>();
                IList<gameobject_loots> ILoots = Database.SelectObjects<gameobject_loots>("Entry=" + entry);
                foreach (gameobject_loots Loot in ILoots)
                    Loots.Add(Loot);

                GameObjectLoots.Add(entry, Loots);

                long MissingGameObject = 0;
                long MissingItemProto = 0;

                if (GetGameObjectProto(entry) == null)
                {
                    Log.Debug("LoadLoots", "[" + entry + "] Invalid GameObject Proto");
                    ++MissingGameObject;
                }

                foreach (gameobject_loots Loot in GameObjectLoots[entry].ToArray())
                {
                    Loot.Info = ItemService.GetItem_Info(Loot.ItemId);

                    if (Loot.Info == null)
                    {
                        Log.Debug("LoadLoots", "[" + Loot.ItemId + "] Invalid Item Info");
                        GameObjectLoots[entry].Remove(Loot);
                        ++MissingItemProto;
                    }
                }

                if (MissingItemProto > 0)
                    Log.Error("LoadLoots", "[" + MissingItemProto + "] Missing Item Info");

                if (MissingGameObject > 0)
                    Log.Error("LoadLoots", "[" + MissingGameObject + "] Misssing GameObject proto");
            }
        }

        public static List<gameobject_loots> GetGameObjectLoots(uint Entry)
        {
            LoadGameObjectLoots(Entry);

            List<gameobject_loots> Loots;

            if (!GameObjectLoots.TryGetValue(Entry, out Loots))
                Loots = new List<gameobject_loots>();

            return Loots;
        }

        #endregion GameObjectLoots
    }
}