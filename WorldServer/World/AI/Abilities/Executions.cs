using Common;
using FrameWork;
using GameData;
using NLog;
using System.Linq;
using WorldServer.Services.World;
using WorldServer.World.Abilities;
using WorldServer.World.Abilities.Buffs;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;
using Object = System.Object;

namespace WorldServer.World.AI.Abilities
{
    public class Executions
    {
        public Unit Owner { get; }
        public CombatInterface_Npc Combat { get; }
        public ABrain Brain { get; set; }

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Executions(Unit owner, CombatInterface_Npc combat, ABrain brain)
        {
            Owner = owner;
            Combat = combat;
            Brain = brain;
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_STORMVERMIN
        /// </summary>
        public void CleaveSoul()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Cleave Soul", 9552);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_PRIEST
        /// </summary>
        public void RottingFlesh()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Rotting Flesh", 13012);
        }

        public void EmptyMind()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Empty Mind", 12223);
        }

        public void HealingHand()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Healing Hand", 8241);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_CLANRAT
        /// </summary>
        public void RippingGnash()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Ripping Gnash", 12111);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_PLAGUE_MONK
        /// </summary>
        public void PlagueCrush()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Plague Crush", 13561);
        }

        public void Plague()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Plague", 13600);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_PACKMASTER
        /// </summary>
        public void InspiringAttack()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Inspiring Attack", 1364);
        }

        public void DemolishingStrike()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Demolishing Strike", 606);
        }

        public void PowerofthePack()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Power of the Pack", 12073);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_RAT_OGRE_BRUTE
        /// </summary>
        public void HardStomp()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Hard Stomp", 5305);
        }

        public void LaceratingTalons()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Lacerating Talons", 12301);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_RAT_OGRE
        /// </summary>
        public void RippingClaws()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Ripping Claws", 12221);
        }

        public void GroundPound()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Ground Pound", 13652);
        }

        /// <summary>
        /// UNDEAD_SPIRITS_SPIRIT_HOST
        /// </summary>
        public void Chillwind()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Chillwind", 9471);
        }

        public void EtherealHand()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Ethereal Hand", 12451);
        }

        public void BloodCurse()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Blood Curse", 5036);
        }

        /// <summary>
        /// HUMANOIDS_GREENSKINS_GAS_SQUIG
        /// </summary>
        public void SporeCloud()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Spore Cloud", 12);
        }

        public void GoopShootin()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Goop Shootin", 11);
        }

        /// <summary>
        /// HUMANOIDS_GREENSKINS_SPIKED_SQUIG
        /// </summary>
        public void PoisonedSpine()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Poisoned Spine", 8);
        }

        public void SpineFling()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Spine Fling", 7);
        }

        /// <summary>
        /// HUMANOIDS_GREENSKINS_HORNED_SQUIG
        /// </summary>
        public void HeadButt()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Head Butt", 10);
        }

        public void Gore()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Gore", 9);
        }

        /// <summary>
        /// HUMANOIDS_GREENSKINS_SQUIG
        /// </summary>
        public void SquigSqueal()
        {
            Brain.SimpleCast(Owner, Owner, "Squig Squeal", 442);
        }

        public void DeathFromAbove()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Death From Above", 6);
        }

        /// <summary>
        /// MECHANIC_FLAME_TURRET
        /// </summary>
        public void SteamVent()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Steam Vent", 28);
        }

        public void Flamethrower()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Flamethrower", 27);
        }

        /// <summary>
        /// MECHANIC_BOMBARDMENT_TURRET
        /// </summary>
        public void HighExplosiveGrenade()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "High-Explosive Grenade", 25);
        }

        public void ShockGrenade()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Shock Grenade", 24);
        }

        /// <summary>
        /// MECHANIC_GUN_TURRET
        /// </summary>
        public void MachineGun()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Machine Gun", 22);
        }

        public void PenetratingRound()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Penetrating Round", 21);
        }

        /// <summary>
        /// DAEMONS_TZEENTCH_FLAMER
        /// </summary>
        public void FlamesofChange()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Flames of Change", 59);
        }

        public void FlameofTzeentch()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Flame of Tzeentch", 58);
        }

        public void DaemonicFire()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Daemonic Fire", 54);
        }

        /// <summary>
        /// HUMANOIDS_HUMANS_MAGUS
        /// </summary>
        public void FlickeringRedFire()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Flickering Red Fire", 8470);
        }

        /// <summary>
        /// HUMANOIDS_HUMANS_ZEALOT
        /// </summary>
        public void Scourge()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Scourge", 8548);
        }

        public void GleanMagic()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Glean Magic", 8482);
        }

        public void ShatterBlessing()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Shatter Confidence", 8023);
        }

        public void PoisonSpit()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "PoisonSpit", 5414);
        }

        public void PrecisionStrike()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "PrecisionStrike", 8005);
        }

        public void SeepingWound()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Seeping Wound", 8320);
        }

        public void KnockDownTarget()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Downfall", 8346);
        }

        public void PuntTarget()
        {
            Combat.CurrentTarget.ApplyKnockback(Owner, AbilityMgr.GetKnockbackInfo(8329, 0));
        }

        public void Corruption()
        {
            if (Combat.CurrentTarget == null)
                return;
            if (Combat.CurrentTarget is Player)
            {
                var target = ((Player)Combat.CurrentTarget);

                Brain.SimpleCast(Owner, Combat.CurrentTarget, "Corruption", 8400);
            }
        }

        public void Stagger()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Quake", 8349);
        }

        public void BestialFlurry()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "BestialFlurry", 5347);
        }

        public void Whirlwind()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Whirlwind", 5568);
        }

        public void EnfeeblingShout()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Enfeebling Shout", 5575);
        }

        public void Cleave()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Cleave", 13626);
        }

        public void Stomp()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Stomp", 4811);
        }

        public void EnragedBlow()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "EnragedBlow", 8315);
        }

        public void FlingSpines()
        {
            var newTarget = Brain.SetRandomTarget();
            if (newTarget != null)
            {
                Combat.SetTarget(newTarget, TargetTypes.TARGETTYPES_TARGET_ENEMY);
                Brain.SimpleCast(Owner, Combat.CurrentTarget, "FlingSpines", 13089);
            }
        }

        public void Terror()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Terror", 5968);
        }

        public void ThunderingBlow()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "ThunderingBlow", 8424);
        }

        public void ArdentBreath()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "ArdentBreath", 13816);
        }

        public void PlagueAura()
        {
            if (Combat.CurrentTarget == null)
                return;
            if (Combat.CurrentTarget is Player)
            {
                var target = ((Player)Combat.CurrentTarget);

                Brain.SimpleCast(Owner, Combat.CurrentTarget, "PlagueAura", 13660);
            }
        }

        public void CorrosiveVomit()
        { //Heal debuff 50 % 30 sec
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "CorrosiveVomit", 5303);
        }

        public void FoulVomit()
        { //Heal debuff 50 % 30 sec
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "FoulVomit", 12522);
        }

        public void RampantSlash()
        { //Heal debuff 50 % 30 sec
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "RampantSlash", 13660);
        }

        public void InfectiousBite()
        { // dot
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "InfectiousBite", 5700);
        }

        public void LowBlow()
        { // dot
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "LowBlow", 5688);
        }

        /// <summary>
        /// ANIMALS_BEASTS_GREAT_CAT
        /// </summary>
        public void Shred()
        { // Armor debuff
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Shred", 430);
        }

        public void LegTear()
        { // 10 sec snare
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "LegTear", 46);
        }

        public void BatScreech()
        { // 10 sec snare
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "BatScreech", 12012);
        }

        public void DaemonicConsumption()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "DaemonicConsumption", 55);
        }

        public void WarpingEnergy()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "DaemonicConsumption", 56);
        }

        public void WhitefireWebBolt()
        { // knockback and snare
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "WhitefireWebBolt", 4462);
        }

        public void SlimyVomit()
        { // PUKEEE
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "SlimyVomit", 5304);
        }

        public void Maul()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Maul", 48);
        }

        public void Charge()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Charge", 13307);
        }

        public void Bite()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "Bite", 41);
        }

        public void WrithingFangs()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "WrithingFangs", 13097);
        }

        public void GutRipper()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "GutRipper", 49);
        }

        public void DisablingStrike()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "DisablingStrike", 5806);
        }

        public void CripplingBlow()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "CripplingBlow", 5132);
        }

        public void EnvenomedStinger()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "EnvenomedStinger", 12402);
        }

        public void SappingStrike()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "SappingStrike", 20224);
        }

        public void BloodPulse()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "BloodPulse", 5066);
        }

        /// <summary>
        /// HUMANOIDS_SKAVEN_RUNNER
        /// </summary>
        public void ScytheGash()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "ScytheGash", 12591);
        }

        public void SimpleStrike()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "SimpleStrike", 8005);
        }

        public void RuneofStriking()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "RuneofStriking", 1586);
        }

        public void GoreBoar()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "GoreBoar", 438);
        }

        public void GunTurret()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "GunTurret", 1511);
        }

        public void MinorPotionofHealing()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "MinorPotionofHealing", 7871);
        }

        public void ClawSweep()
        {
            Brain.SimpleCast(Owner, Combat.CurrentTarget, "ClawSweep", 42);
        }

        public void BloodscentAura()
        {
            if (Combat.CurrentTarget == null)
                return;
            if (Combat.CurrentTarget is Player)
            {
                var target = ((Player)Combat.CurrentTarget);
                Brain.SimpleCast(Owner, Combat.CurrentTarget, "BloodscentAura", 13728);
            }
        }

        //public void IncrementPhase()
        //{
        //    // Phases must be ints in ascending order.
        //    var currentPhase = CurrentPhase;
        //    if (Phases.Count == currentPhase)
        //        return;
        //    CurrentPhase = currentPhase + 1;

        //    Brain.SpeakYourMind($" using Increment Phase vs {currentPhase}=>{CurrentPhase}");
        //}

        /// <summary>
        /// Aslong as the Banner of Bloodlust i s up,Borzhar will charge a t a new target
        /// and should s tay locked on the target for a medium duration before charging
        /// at a new target. Players can destroy the banner which will prevent him f rom
        /// using this charge anymore.
        /// </summary>
        public void DeployBannerOfBloodlust()
        {
            GameObject_proto proto = GameObjectService.GetGameObjectProto(3100412);

            gameobject_spawns spawn = new gameobject_spawns
            {
                Guid = (uint)GameObjectService.GenerateGameObjectSpawnGUID(),
                WorldX = Owner.WorldPosition.X + StaticRandom.Instance.Next(50),
                WorldY = Owner.WorldPosition.Y + StaticRandom.Instance.Next(50),
                WorldZ = Owner.WorldPosition.Z,
                WorldO = Owner.Heading,
                ZoneId = Owner.Zone.ZoneId
            };

            spawn.BuildFromProto(proto);
            proto.CreatureSpawnIsAttackable = 1;

            var go = Owner.Region.CreateGameObject(spawn);
            go.EvtInterface.AddEventNotify(EventName.OnDie, RemoveGOs);
        }

        private bool RemoveGOs(Object obj, object args)
        {
            GameObject go = obj as GameObject;
            go.EvtInterface.AddEvent(go.Destroy, 2 * 1000, 1);
            return false;
        }

        /// <summary>
        /// Aslong as the Banner of the Bloodherdisup, Bloodherd Gors willrally to
        /// Borzhar’s side. To stopthe reinforcement, players must destroy the Banner of
        /// the Bloodherd.
        /// </summary>
        public void BannerOfTheBloodHerd()
        {
            // If the Banner exists within 150 feet, allow spawn adds
            var creatures = Owner.GetInRange<GameObject>(150);
            foreach (var creature in creatures)
            {
                if (creature.Entry == 3100412)
                {
                    SpawnAdds();
                    break;
                }
            }
        }

        public void SpawnAdds()
        {
            if (Owner is Boss)
            {
                var adds = (Owner as Boss).AddDictionary;

                foreach (var entry in adds)
                {
                    Spawn(entry);
                }

                // Force zones to update
                Owner.Region.Update();
            }
        }

        public void EnergyFlux()
        {
            // Brain.SpeakYourMind(" using EnergyFlux");

            // Remove any old electron fluxes (max of 4)
            var fluxes = Owner.GetInRange<GameObject>(150);

            if (fluxes.Count > 4)
            {
                foreach (var flux in fluxes)
                {
                    if (flux.Entry == 3100414)
                    {
                        flux.EvtInterface.AddEvent(flux.Destroy, 2 * 1000, 1);
                        break;
                    }
                }
            }

            GameObject_proto proto = GameObjectService.GetGameObjectProto(3100414);

            var newTarget = Brain.SetRandomTarget();
            if (newTarget != null)
            {
                gameobject_spawns spawn = new gameobject_spawns
                {
                    Guid = (uint)GameObjectService.GenerateGameObjectSpawnGUID(),
                    WorldO = 2093,
                    WorldX = newTarget.WorldPosition.X + StaticRandom.Instance.Next(50),
                    WorldY = newTarget.WorldPosition.Y + StaticRandom.Instance.Next(50),
                    WorldZ = newTarget.WorldPosition.Z,
                    ZoneId = (ushort)Owner.ZoneId
                };

                spawn.BuildFromProto(proto);
                proto.CreatureSpawnIsAttackable = 1;

                var go = Owner.Region.CreateGameObject(spawn);
                // When the gameobject dies, remove it.
                go.EvtInterface.AddEventNotify(EventName.OnDie, RemoveGOs);
                // When the boss dies, remove all child "fluxes"
                Owner.EvtInterface.AddEventNotify(EventName.OnDie, RemoveAllFluxes);
                // Buff the flux with the lightning rod buff.
                go.BuffInterface.QueueBuff(new BuffQueueInfo(go, 48, AbilityMgr.GetBuffInfo((ushort)1543),
                    BuffAssigned));
            }
        }

        private bool RemoveAllFluxes(Objects.Object obj, object args)
        {
            var fluxes = Owner.GetInRange<GameObject>(150);

            foreach (var flux in fluxes)
            {
                if (flux.Entry == 3100414)
                {
                    flux.EvtInterface.AddEvent(flux.Destroy, 2 * 1000, 1);
                }
            }

            return true;
        }

        private void Spawn(BossSpawn entry)
        {
            ushort facing = 2093;

            var X = Owner.WorldPosition.X;
            var Y = Owner.WorldPosition.Y;
            var Z = Owner.WorldPosition.Z;

            var spawn = new creature_spawns { Guid = (uint)CreatureService.GenerateCreatureSpawnGUID() };
            var proto = CreatureService.GetCreatureProto(entry.ProtoId);
            if (proto == null)
                return;
            spawn.BuildFromProto(proto);

            spawn.WorldO = facing;
            spawn.WorldX = X + StaticRandom.Instance.Next(500);
            spawn.WorldY = Y + StaticRandom.Instance.Next(500);
            spawn.WorldZ = Z;
            spawn.ZoneId = (ushort)Owner.ZoneId;

            var creature = Owner.Region.CreateCreature(spawn);
            creature.EvtInterface.AddEventNotify(EventName.OnDie, RemoveNPC);
            entry.Creature = creature;
            (Owner as Boss).SpawnDictionary.Add(entry);

            if (entry.Type == BrainType.AggressiveBrain)
                creature.AiInterface.SetBrain(new AggressiveBrain(creature));
            if (entry.Type == BrainType.HealerBrain)
                creature.AiInterface.SetBrain(new HealerBrain(creature));
            if (entry.Type == BrainType.PassiveBrain)
                creature.AiInterface.SetBrain(new PassiveBrain(creature));
        }

        private bool RemoveNPC(Object obj, object args)
        {
            Creature c = obj as Creature;
            if (c != null) c.EvtInterface.AddEvent(c.Destroy, 20000, 1);

            return false;
        }

        private void BuffAssigned(NewBuff buff)
        {
            var newBuff = buff;
        }

        private void SwitchToLowHealthTarget()
        {
            // Go for Low Health target
            var enemyPlayers = Owner.GetPlayersInRange(30, false).Where(x => x.Realm != Owner.Realm)
                .ToList();
            if (enemyPlayers.Count() > 0)
            {
                foreach (var enemyPlayer in enemyPlayers)
                {
                    if (enemyPlayer.PctHealth < 50)
                    {
                        _logger.Debug($"{Owner} changing target to  {(enemyPlayer as Player).Name}");
                        Owner.CbtInterface.SetTarget(enemyPlayer.Oid,
                            TargetTypes.TARGETTYPES_TARGET_ENEMY);
                        break;
                    }
                }
            }
        }

        private void SwitchTarget()
        {
            // Switch targets
            if (Combat.GetCurrentTarget() is Player)
            {
                _logger.Debug($"{Owner} using Changing Targets {(Combat.GetCurrentTarget() as Player).Name}");
                var randomTarget = Owner.AiInterface.CurrentBrain.SetRandomTarget();
                if (randomTarget != null)
                    _logger.Debug($"{Owner} => {randomTarget.Name}");
            }
        }
    }
}