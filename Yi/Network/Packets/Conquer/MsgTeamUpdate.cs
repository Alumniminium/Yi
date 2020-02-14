using System;
using System.Runtime.InteropServices;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Sockets;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgTeamUpdate
    {
        public ushort Size;
        public ushort Id;
        public MsgTeamMemberAction Action;
        public byte Amount;
        public short Unknown;
        public fixed byte TargetName [16];
        public int UniqueId;
        public uint Look;
        public ushort MaxHp;
        public ushort CurHp;

        public static MsgTeamUpdate JoinLeave(YiObj owner, MsgTeamMemberAction addMember)
        {
            var packet = new MsgTeamUpdate
            {
                Size = (ushort)sizeof(MsgTeamUpdate),
                Id = 1026, Action = addMember, Amount = 1, Unknown = 16, UniqueId = owner.UniqueId, Look = owner.Look, MaxHp = (ushort)Math.Max(owner.MaximumHp, owner.CurrentHp), CurHp = (ushort)owner.CurrentHp};
            for (byte i = 0; i < owner.Name.Length; i++)
                packet.TargetName[i] = (byte)owner.Name[i];

            return packet;
        }

        public static implicit operator byte[](MsgTeamUpdate msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgTeamUpdate*)p = *&msg;
            return buffer;
        }
    }
}