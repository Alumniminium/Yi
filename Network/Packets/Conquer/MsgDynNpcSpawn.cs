using System.Runtime.InteropServices;
using YiX.Network.Sockets;
using DynamicNpc = YiX.Entities.DynamicNpc;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 101)]
    public unsafe struct MsgDynNpcSpawn
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public uint MaximumHp;
        public uint CurrentHp;
        public short X;
        public short Y;
        public ushort Look;
        public ushort Type;
        public ushort Base;

        public static byte[] Create(DynamicNpc npc)
        {
            var packet = new MsgDynNpcSpawn
            {
                Size = (ushort)sizeof(MsgDynNpcSpawn),
                Id = 1109,
                UniqueId = npc.UniqueId,
                CurrentHp = (ushort)npc.CurrentHp,
                MaximumHp = npc.MaximumHp,
                X = (short) npc.Location.X,
                Y = (short) npc.Location.Y,
                Look = (ushort)npc.Look,
                Type = npc.Type, Base = npc.Base
            };
            return packet;
        }

        public static implicit operator byte[](MsgDynNpcSpawn msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgDynNpcSpawn*)p = *&msg;
            return buffer;
        }
    }
}