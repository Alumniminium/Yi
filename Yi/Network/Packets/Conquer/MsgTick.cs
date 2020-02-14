using System;
using System.Runtime.InteropServices;
using System.Text;
using Yi.Entities;
using Yi.Network.Sockets;
using Player = Yi.Entities.Player;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgTick
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public int Timestamp;
        public uint Junk1;
        public uint Junk2;
        public uint Junk3;
        public uint Junk4;
        public uint Hash;

        public static byte[] Create(YiObj target)
        {
            var packet = stackalloc MsgTick[1];
            packet->Size = (ushort) sizeof(MsgTick);
            packet->Id = 1012;
            packet->UniqueId = target.UniqueId;
            packet->Timestamp = Environment.TickCount + 100000;
            packet->Junk1 = 0;
            packet->Junk2 = 0;
            packet->Junk3 = 0;
            packet->Junk4 = 0;
            packet->Hash = 0;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgTick*)p = *packet;
            return buffer;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            fixed (byte* p = buffer)
            {
                var packet = *(MsgTick*) p;
                BufferPool.RecycleBuffer(buffer);

                packet.Timestamp = packet.Timestamp ^ packet.UniqueId;

                if (player.UniqueId != packet.UniqueId)
                    Output.WriteLine($"UID Mismatch! {packet.UniqueId}");

                if (player.LastTick == 0)
                    player.LastTick = packet.Timestamp;

                if (packet.Hash != HashName(player.Name.TrimEnd('\0')))
                    Output.WriteLine($"Hash Mismatch! {packet.Hash}");

                player.LastTick = packet.Timestamp;
            }
        }

        private static uint HashName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 4)
                return 0x9D4B5703;

            var buffer = Encoding.GetEncoding("iso-8859-1").GetBytes(name);
            fixed (byte* pBuf = buffer)
                return ((ushort*)pBuf)[0] ^ 0x9823U;
        }

        public static implicit operator byte[](MsgTick msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgTick*)p = *&msg;
            return buffer;
        }
    }
}