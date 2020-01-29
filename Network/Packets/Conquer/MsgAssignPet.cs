using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgAssignPet
    {
        public ushort Size;
        public ushort Id;
        public int UnqiueId;
        public uint Model;
        public uint AI;
        public ushort X;
        public ushort Y;
        public fixed byte Summoner [16];

        public static byte[] Create(YiObj owner, int uid)
        {
            var packet = new MsgAssignPet
            {
                Size = 36,
                Id = 2035,
                Model = 920,
                AI = 1,
                X = owner.Location.X,
                Y = owner.Location.Y,
                UnqiueId = uid,
            };
            for (byte i = 0; i < "GoldGuard".Length; i++)
                packet.Summoner[i] = (byte)"GoldGuard"[i];

            return packet;
        }

        public static implicit operator byte[](MsgAssignPet msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgAssignPet*)p = *&msg;
            return buffer;
        }
    }
}