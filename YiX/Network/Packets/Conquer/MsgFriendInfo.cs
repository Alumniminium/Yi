using System.Runtime.InteropServices;
using YiX.Network.Sockets;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgFriendInfo
    {
        public ushort Size;
        public ushort Id;
        public int UniqId;
        public uint Look;
        public byte Level;
        public byte Profession;
        public ushort PkPoints;
        public ushort GuildUID;
        public byte Unknow;
        public byte Position;
        public fixed byte Spouse [16];

        public static byte[] Create(Player target)
        {
            var packet = new MsgFriendInfo
            {
                Size = (ushort)sizeof(MsgFriendInfo),
                Id = 2033,
                UniqId = target.UniqueId,
                Look = target.Look,
                Level = target.Level,
                Profession = target.Class,
                PkPoints = target.PkPoints,
                GuildUID = 0,
                Unknow = 0,
                Position = (byte)target.GuildRank,
            };

            for (byte i = 0; i < target.Partner.Length; i++)
                packet.Spouse[i] = (byte)target.Partner[i];
            return packet;
        }

        public static implicit operator byte[](MsgFriendInfo msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgFriendInfo*)p = *&msg;
            return buffer;
        }
    }
}