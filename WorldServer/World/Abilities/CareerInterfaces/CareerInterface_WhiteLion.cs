using Common;
using System;
using WorldServer.World.Abilities.Buffs;
using WorldServer.World.Objects;
using WorldServer.World.Positions;

namespace WorldServer.World.Abilities.CareerInterfaces
{
    internal class CareerInterface_WhiteLion : CareerInterface, IPetCareerInterface
    {
        public Pet myPet
        { get { return _myPet; } set { _myPet = value; } }

        private byte _AIMode = 5;

        // This is variable that store current pet health
        private uint currentHealth;

        public byte AIMode
        { get { return _AIMode; } set { _AIMode = value; } }

        private ushort _currentPetID;

        public string MyPetName { get; set; }

        private static readonly ushort[] _petTrainBuffs = { 3954, 3955, 3951 };

        public CareerInterface_WhiteLion(Player player) : base(player)
        {
            _resourceTimeout = 0;
        }

        public override bool HasResource(byte amount)
        {
            return true;
        }

        // Used to set up pet buffs which should be reapplied if the lion's resummoned
        public override void SetResource(byte amount, bool blockEvent)
        {
            if (amount == 0 || amount == _careerResource)
                return;
            _careerResource = amount;

            if (myPet != null && !myPet.IsDead)
            {
                currentHealth = myPet.Health;
                myPet.BuffInterface.QueueBuff(new BuffQueueInfo(myPlayer, myPlayer.EffectiveLevel, AbilityMgr.GetBuffInfo(_petTrainBuffs[amount - 1], myPlayer, myPet)));
                myPet.EvtInterface.AddEvent(SetHealth, 100, 1);
            }
        }

        private void SetHealth()
        {
            if (myPet != null)
                myPet.Health = currentHealth;
        }

        public override void SendResource()
        {
        }

        private bool _summoning;
        private Pet _myPet;

        public void SummonPet(ushort myID)
        {
            if (_summoning)
                return;
            try
            {
                _summoning = true; // Happens when pet is automatically reset after zone change
                if (myPet != null)
                {
                    myPet.ReceiveDamage(myPet, uint.MaxValue);
                    myPet = null;
                }
                if (myPlayer.Zone == null)
                    return;

                _currentPetID = myID;

                Creature_proto Proto = new Creature_proto { Faction = 65 };

                if (myID == 9159)
                {
                    if (myPlayer.Level < 16)
                        Proto.Name = myPlayer.Name + "'s Lion Cub";
                    else
                        Proto.Name = myPlayer.Name + "'s War Lion";
                    Proto.Model1 = (ushort)(132 + ((myPlayer.Level - 1) * 0.1f)); ;
                    Proto.Ranged = 10;
                }

                if (myPlayer.Info.PetModel != 0)
                {
                    Proto.Model1 = myPlayer.Info.PetModel;
                }

                if (!string.IsNullOrEmpty(myPlayer.Info.PetName))
                    MyPetName = myPlayer.Info.PetName;

                creature_spawns Spawn = new creature_spawns();
                Proto.MinScale = 50;
                Proto.MaxScale = 50;
                Spawn.BuildFromProto(Proto);
                Spawn.WorldO = myPlayer._Value.WorldO;
                Point3D offset = WorldUtils.GetForward(myPlayer, 75);
                Spawn.WorldX = myPlayer._Value.WorldX + offset.X;
                Spawn.WorldY = myPlayer._Value.WorldY + offset.Y;
                Spawn.WorldZ = myPlayer._Value.WorldZ;
                Spawn.ZoneId = myPlayer.Zone.ZoneId;
                Spawn.Icone = 18;
                Spawn.WaypointType = 0;
                Spawn.Proto.MinLevel = Spawn.Proto.MaxLevel = myPlayer.EffectiveLevel;

                if (Spawn.Proto.MinLevel > 40)
                {
                    Spawn.Proto.MinLevel = 40;
                    Spawn.Proto.MaxLevel = 40;
                }

                myPet = new Pet(myID, Spawn, myPlayer, AIMode, false, true);

                myPlayer.Region.AddObject(myPet, Spawn.ZoneId);

                if (_careerResource != 0)
                {
                    currentHealth = myPet.Health;
                    myPet.BuffInterface.QueueBuff(new BuffQueueInfo(myPlayer, myPlayer.EffectiveLevel, AbilityMgr.GetBuffInfo(_petTrainBuffs[_careerResource - 1], myPlayer, myPet)));
                    myPet.EvtInterface.AddEvent(SetHealth, 100, 1);
                }

                myPlayer.BuffInterface.NotifyPetEvent(myPet);
            }
            finally
            {
                _summoning = false;
            }
        }

        public void Notify_PetDown()
        {
            if (myPet != null)
            {
                myPlayer.AbtInterface.SetCooldown(9159, 15000);
                myPlayer.BuffInterface.NotifyPetEvent(myPet);
                myPet = null;
            }
        }

        public override void Stop()
        {
            if (myPet != null)
            {
                myPet.Destroy();
                myPet = null;
            }

            base.Stop();
        }

        public override Unit GetTargetOfInterest()
        {
            return myPet;
        }

        public override EArchetype GetArchetype()
        {
            return EArchetype.ARCHETYPE_DPS;
        }
    }
}