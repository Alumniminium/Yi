using System.Runtime.InteropServices;
using Yi.Network.Sockets;
using Yi.Structures;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgProf
    {
        public ushort Size;
        public ushort Id;
        public uint ProfId;
        public uint Level;
        public uint Experience;

        public static byte[] Create(Prof prof)
        {
            var packet = new MsgProf
            {
                Size = (ushort)sizeof(MsgProf), Id = 1025, ProfId = prof.Id, Experience = prof.Experience, Level = prof.Level};
            return packet;
        }

        public static unsafe implicit operator byte[](MsgProf msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgProf*)p = *&msg;
            return buffer;
        }
    }
}