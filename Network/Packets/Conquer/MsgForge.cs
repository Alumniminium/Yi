using System;
using System.Collections.Generic;
using YiX.Network.Sockets;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    public static class MsgForge
    {
        public static Player Player;
        public static ushort Amount;
        public static readonly HashSet<uint> UniqueIds = new HashSet<uint>();

        public static void Handle(Player player, byte[] packet)
        {
            Player = player;
            Amount = BitConverter.ToUInt16(packet, 10);

            for (var i = 0; i < Amount; i++)
                UniqueIds.Add(BitConverter.ToUInt32(packet, 12 + i * 4));

            Process();
            BufferPool.RecycleBuffer(packet);
        }

        private static void Process()
        {
        }
    }
}