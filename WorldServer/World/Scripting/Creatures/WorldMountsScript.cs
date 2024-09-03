using FrameWork;
using WorldServer.NetWork.Handler;
using WorldServer.World.Abilities.Components;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;
using WorldServer.World.Scripting.Mounts;

namespace WorldServer.World.Scripting.Creatures
{
    [GeneralScript(false, "", CreatureEntry = 116, GameObjectEntry = 0)]
    public class WorldOrderMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        public static void Mount(Player Target)
        {
            if (Target.MvtInterface.HasUnit())
                return;

            if (Target.Info.Race == (byte)GameData.Races.RACES_DWARF)
                Target.MvtInterface.CurrentMount.SetMount(8);
            else if (StaticRandom.Instance.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(180);
            else
                Target.MvtInterface.CurrentMount.SetMount(1);
        }
    }

    [GeneralScript(false, "", CreatureEntry = 155, GameObjectEntry = 0)]
    public class WorldDestructionMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        public static void Mount(Player Target)
        {
            if (Target.MvtInterface.CurrentMount.IsMount())
                return;

            if (StaticRandom.Instance.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(3);
            else
                Target.MvtInterface.CurrentMount.SetMount(12);
        }
    }

    [GeneralScript(true, "WorldFleeAbility")]
    public class WorldFleeAbilityMount : AGeneralScript
    {
        public override void OnCastAbility(AbilityInfo Ab)
        {
            if (Ab.Caster.IsPlayer() && Ab.ConstantInfo.Entry == 245) // Flee
            {
                if (Ab.Caster.GetPlayer().MvtInterface.CurrentMount.IsMount())
                {
                    Ab.Caster.GetPlayer().MvtInterface.CurrentMount.UnMount();
                    return;
                }

                if (Ab.Caster.GetPlayer().Realm == GameData.SetRealms.REALMS_REALM_ORDER)
                    WorldOrderMountScript.Mount(Ab.Caster.GetPlayer());
                else
                    WorldDestructionMountScript.Mount(Ab.Caster.GetPlayer());
            }
        }
    }
}