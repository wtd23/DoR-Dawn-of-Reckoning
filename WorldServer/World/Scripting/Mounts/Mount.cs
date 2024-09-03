using Common;
using FrameWork;
using WorldServer.Managers;
using WorldServer.World.Interfaces;
using WorldServer.World.Objects;
using Object = WorldServer.World.Objects.Object;

namespace WorldServer.World.Scripting.Mounts
{
    public class Mount
    {
        public Unit Owner;
        public mount_infos CurrentMountInfo;

        public Mount(Unit Owner)
        {
            this.Owner = Owner;
            Owner.EvtInterface.AddEventNotify(EventName.OnReceiveDamage, OnTakeDamage);
            Owner.EvtInterface.AddEventNotify(EventName.OnDealDamage, OnDealDamage);
            Owner.EvtInterface.AddEventNotify(EventName.OnStartCasting, OnStartCast);
            Owner.EvtInterface.AddEventNotify(EventName.OnRemoveFromWorld, OnRemoveFromWorld);
        }

        public void Stop()
        {
        }

        public bool IsMount()
        {
            return CurrentMountInfo != null;
        }

        public void SetMount(uint Id)
        {
            mount_infos Info = WorldMgr.GetMount(Id);
            SetMount(Info);
        }

        public void SetMount(mount_infos Info)
        {
            UnMount();
            if (Info == null)
                return;

            CurrentMountInfo = Info;
            Owner.StsInterface.AddBonusSpeed(CurrentMountInfo.Speed);
            SendMount(null);
        }

        public void UnMount()
        {
            if (CurrentMountInfo == null)
                return;

            Owner.StsInterface.RemoveBonusSpeed(CurrentMountInfo.Speed);
            CurrentMountInfo = null;
            SendMount(null);
        }

        public void SendMount(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MOUNT_UPDATE);
            Out.WriteUInt16(Owner.Oid);

            if (CurrentMountInfo == null)
                Out.WriteUInt32(0);
            else
            {
                Out.WriteInt16((short)CurrentMountInfo.Entry);
                Out.WriteByte(0);
                Out.WriteByte(0);
            }
            Out.Fill(0, 14);

            if (Plr == null)
                Owner.DispatchPacket(Out, true);
            else
                Plr.SendPacket(Out);
        }

        public bool OnStartCast(Object Obj, object Args)
        {
            UnMount();
            return false;
        }

        public bool OnTakeDamage(Object Obj, object Args)
        {
            UnMount();
            return false;
        }

        public bool OnDealDamage(Object Obj, object Args)
        {
            UnMount();
            return false;
        }

        public bool OnRemoveFromWorld(Object Obj, object Args)
        {
            UnMount();
            return false;
        }
    }
}