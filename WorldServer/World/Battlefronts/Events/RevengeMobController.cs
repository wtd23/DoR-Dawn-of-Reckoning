using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using WorldServer.Managers;
using WorldServer.Services.World;
using WorldServer.World.Map;
using WorldServer.World.Objects;

namespace WorldServer.World.Battlefronts.Events
{
    public class RevengeMobController
    {
        public ConcurrentDictionary<int, MobPosition> MobPathway { get; set; }
        public ConcurrentDictionary<int, MobPosition> MobSpawn { get; set; }
        public List<creature_spawns> CreatureSpawnList { get; set; }
        public List<Creature> ActiveCreatureList { get; set; }

        public RegionMgr RegionMgr { get; set; }
        public int ZoneId { get; set; }

        public RevengeMobController()
        {
            MobPathway = new ConcurrentDictionary<int, MobPosition>();
            MobSpawn = new ConcurrentDictionary<int, MobPosition>();
            CreatureSpawnList = new List<creature_spawns>();
            ActiveCreatureList = new List<Creature>();
        }

        public void SetPathway()
        {
            MobPathway.TryAdd(1, new MobPosition(1448651, 856214, 14936, 2118));
            MobPathway.TryAdd(2, new MobPosition(1446650, 854191, 14904, 2118));
            MobPathway.TryAdd(3, new MobPosition(1448270, 852754, 15263, 2118));
            MobPathway.TryAdd(4, new MobPosition(1449686, 852649, 15543, 2118));
            MobPathway.TryAdd(5, new MobPosition(1450167, 851857, 15537, 2118));
            MobPathway.TryAdd(6, new MobPosition(1450128, 850670, 15455, 2118));
        }

        public void SetInitialLocation()
        {
            MobSpawn.TryAdd(1, new MobPosition(1448651, 856214, 14936, 2118));
            MobSpawn.TryAdd(2, new MobPosition(1448622, 856224, 14936, 2118));
            MobSpawn.TryAdd(3, new MobPosition(1448574, 856245, 14936, 2118));
            MobSpawn.TryAdd(4, new MobPosition(1448522, 856261, 14936, 2118));
        }

        public void Initialise()
        {
            if (MobPathway.Count == 0)
                throw new Exception("MobPathway not set");
            CreatureSpawnList.Add(CreateCreatureSpawn(2349, 1));
            CreatureSpawnList.Add(CreateCreatureSpawn(2349, 2));
            CreatureSpawnList.Add(CreateCreatureSpawn(2349, 3));
            CreatureSpawnList.Add(CreateCreatureSpawn(2349, 4));
        }

        public void PlaceMobSpawns()
        {
            foreach (var creatureSpawn in CreatureSpawnList)
            {
                var creature = RegionMgr.CreateCreature(creatureSpawn);

                ActiveCreatureList.Add(creature);
            }
        }

        public void StartMovement()
        {
            foreach (var mobPosition in MobPathway)
            {
                foreach (var creature in ActiveCreatureList)
                {
                    creature.UpdateWorldPosition();
                    creature.MvtInterface.Move((int)mobPosition.Value.X, (int)mobPosition.Value.Y, (int)mobPosition.Value.Z);
                    creature.Aggressive = true;
                    creature.Level = 50;// 30
                }
            }
        }

        public creature_spawns CreateCreatureSpawn(int createProtoId, int mobId)
        {
            Creature_proto proto = CreatureService.GetCreatureProto((uint)createProtoId) ?? WorldMgr.Database.SelectObject<Creature_proto>("Entry=" + createProtoId);

            var spawn = new creature_spawns();
            spawn.Guid = (uint)CreatureService.GenerateCreatureSpawnGUID();
            spawn.BuildFromProto(proto);
            spawn.WorldO = (int)this.MobSpawn[mobId].O;
            spawn.WorldY = (int)this.MobSpawn[mobId].Y;
            spawn.WorldZ = (int)this.MobSpawn[mobId].Z;
            spawn.WorldX = (int)this.MobSpawn[mobId].X;
            spawn.ZoneId = (ushort)ZoneId;
            spawn.Enabled = 1;

            WorldMgr.Database.AddObject(spawn);

            return spawn;
        }
    }

    public class MobPosition
    {
        public MobPosition(uint v1, uint v2, uint v3, uint v4)
        {
            this.O = v4;
            this.X = v1;
            this.Y = v2;
            this.Z = v3;
        }

        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }
        public uint O { get; set; }
    }
}