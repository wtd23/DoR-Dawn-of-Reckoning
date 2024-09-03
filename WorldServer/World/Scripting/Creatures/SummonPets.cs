using Common;
using FrameWork;
using System.Collections.Generic;
using WorldServer.NetWork.Handler;
using WorldServer.World.AI.Abilities;
using WorldServer.Services.World;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;
using Object = WorldServer.World.Objects.Object;
using Opcodes = WorldServer.NetWork.Opcodes;

namespace WorldServer.World.Scripting.Npc
{
    /// <summary>
    /// Magus Summon Pink Horror
    /// </summary>
    [GeneralScript(true, "SummonPinkHorror", 0, 0)]
    public class SummonPinkHorror : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint PinkHorror = 190001; // Pink Horror Entry
            Creature_proto PinkHorrorNew = CreatureService.GetCreatureProto(PinkHorror);
            if (PinkHorrorNew == null)
                return;

            PinkHorrorNew.Name = creature.Name + "'s Pink Horror^M";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(PinkHorrorNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Magus Summon Blue Horror
    /// </summary>
    [GeneralScript(true, "SummonBlueHorror", 0, 0)]
    public class SummonBlueHorror : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint BlueHorror = 670001; // Blue Horror Entry
            Creature_proto BlueHorrorNew = CreatureService.GetCreatureProto(BlueHorror);
            if (BlueHorrorNew == null)
                return;

            BlueHorrorNew.Name = creature.Name + "'s Blue Horror^M";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(BlueHorrorNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Magus Summon Flamer
    /// </summary>
    [GeneralScript(true, "SummonFlamer", 0, 0)]
    public class SummonFlamer : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint Flamer = 1371001; // Flamer Entry
            Creature_proto FlamerNew = CreatureService.GetCreatureProto(Flamer);
            if (FlamerNew == null)
                return;

            FlamerNew.Name = creature.Name + "'s Flamer^M";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(FlamerNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Engineer Summon Gun Turret
    /// </summary>
    [GeneralScript(true, "SummonGunTurret", 0, 0)]
    public class SummonGunTurret : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint GunTurret = 510001; // Gun Turret Entry
            Creature_proto GunTurretNew = CreatureService.GetCreatureProto(GunTurret);
            if (GunTurretNew == null)
                return;

            GunTurretNew.Name = creature.Name + "'s Gun Turret^N";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(GunTurretNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Engineer Summon Bombardment Turret
    /// </summary>
    [GeneralScript(true, "SummonBombardmentTurret", 0, 0)]
    public class SummonBombardmentTurret : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint BombardmentTurret = 7390001; // Bombardment Turret Entry
            Creature_proto BombardmentTurretNew = CreatureService.GetCreatureProto(BombardmentTurret);
            if (BombardmentTurretNew == null)
                return;

            BombardmentTurretNew.Name = creature.Name + "'s Bombardment Turret^N";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(BombardmentTurretNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Engineer Summon Flame Turret
    /// </summary>
    [GeneralScript(true, "SummonFlameTurret", 0, 0)]
    public class SummonFlameTurret : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint FlameTurret = 6770001; // Flame Turret Entry
            Creature_proto FlameTurretNew = CreatureService.GetCreatureProto(FlameTurret);
            if (FlameTurretNew == null)
                return;

            FlameTurretNew.Name = creature.Name + "'s Flame Turret^N";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(FlameTurretNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Squig Herder Summon Squig
    /// </summary>
    [GeneralScript(true, "SummonSquig", 0, 0)]
    public class SummonSquig : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint Squig = 990001; // Squig Entry
            Creature_proto SquigNew = CreatureService.GetCreatureProto(Squig);
            if (SquigNew == null)
                return;

            SquigNew.Name = creature.Name + "'s Squig";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(SquigNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Squig Herder Summon Horned Squig
    /// </summary>
    [GeneralScript(true, "SummonHornedSquig", 0, 0)]
    public class SummonHornedSquig : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint HornedSquig = 2780001; // Horned Squig Entry
            Creature_proto HornedSquigNew = CreatureService.GetCreatureProto(HornedSquig);
            if (HornedSquigNew == null)
                return;

            HornedSquigNew.Name = creature.Name + "'s Horned Squig";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(HornedSquigNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Squig Herder Summon Spiked Squig
    /// </summary>
    [GeneralScript(true, "SummonSpikedSquig", 0, 0)]
    public class SummonSpikedSquig : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint SpikedSquig = 13090001; // Spiked Squig Entry
            Creature_proto SpikedSquigNew = CreatureService.GetCreatureProto(SpikedSquig);
            if (SpikedSquigNew == null)
                return;

            SpikedSquigNew.Name = creature.Name + "'s Spiked Squig";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(SpikedSquigNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// Squig Herder Summon Gas Squig
    /// </summary>
    [GeneralScript(true, "SummonGasSquig", 0, 0)]
    public class SummonGasSquig : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint GasSquig = 2560001; // Gas Squig Entry
            Creature_proto GasSquigNew = CreatureService.GetCreatureProto(GasSquig);
            if (GasSquigNew == null)
                return;

            GasSquigNew.Name = creature.Name + "'s Gas Squig";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(GasSquigNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// White Lion Summon Lion Cub
    /// </summary>
    [GeneralScript(true, "SummonLionCub", 0, 0)]
    public class SummonLionCub : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint LionCub = 21660001; // Lion Cub Entry
            Creature_proto LionCubNew = CreatureService.GetCreatureProto(LionCub);
            if (LionCubNew == null)
                return;

            LionCubNew.Name = creature.Name + "'s Lion Cub";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(LionCubNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }

    /// <summary>
    /// White Lion Summon War Lion
    /// </summary>
    [GeneralScript(true, "SummonWarLion", 0, 0)]
    public class SummonWarLion : AGeneralScript
    {
        public override void OnObjectLoad(Object Obj)
        {
            Creature creature = Obj as Creature;

            uint WarLion = 6630001; // War Lion Entry
            Creature_proto WarLionNew = CreatureService.GetCreatureProto(WarLion);
            if (WarLionNew == null)
                return;

            WarLionNew.Name = creature.Name + "'s War Lion";

            Obj.UpdateWorldPosition();
            creature_spawns Spawn = new creature_spawns(WarLionNew,
                                                      (uint)CreatureService.GenerateCreatureSpawnGUID(),
                                                      Obj.Zone.ZoneId,
                                                      Obj.WorldPosition.X,
                                                      Obj.WorldPosition.Y + 100,
                                                      Obj.WorldPosition.Z,
                                                      Obj.Heading)
            {
                Level = creature.Level
            };
            Creature cr = Obj.Region.CreateCreature(Spawn);
            cr.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAdds);
        }
    }
}