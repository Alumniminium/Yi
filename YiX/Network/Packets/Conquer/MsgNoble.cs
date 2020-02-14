using System.Runtime.InteropServices;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgNoble
    {
        public ushort Size;
        public ushort Id;
        public uint Action;
        public uint Param;
        //public uint Type;

        public static byte[] Create(uint action,uint param)
        {
            var packet = new MsgNoble
            {
                Size = (ushort)sizeof(MsgNoble),
                Id = 2064,
                Action = action,
                Param = param
            };
            return packet;
        }

        public static unsafe implicit operator byte[] (MsgNoble msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgNoble*)p = *&msg;
            return buffer;
        }
    }
}