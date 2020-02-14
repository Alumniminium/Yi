using System.Runtime.InteropServices;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Sockets;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgNpc
    {
        public ushort Size;
        public ushort Id;
        public int UniqId;//4
        public int Param;//8
        public MsgNpcAction Action;//12
        public short Type;//14

        public static byte[] Create(YiObj target, ushort param, MsgNpcAction action)
        {
            var packet = new MsgNpc
            {
                Size = (ushort)sizeof(MsgNpc),
                Id = 2031,
                UniqId = target.UniqueId,
                Param = param,
                Action = action,
                Type=26
            };
            return packet;
        }

        public static unsafe implicit operator byte[](MsgNpc msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgNpc*)p = *&msg;
            return buffer;
        }
    }
}