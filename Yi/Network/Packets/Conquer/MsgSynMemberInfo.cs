using System;
using System.Runtime.InteropServices;
using System.Text;
using Yi.Enums;
using Yi.Network.Sockets;
using Yi.World;
using Player = Yi.Entities.Player;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgSynMemberInfo
    {
        public ushort Size;
        public ushort Id;
        public int Donation;
        public GuildRanks Rank;
        public fixed byte Name [16];

        public static byte[] Create(Player member)
        {
            var packet = new MsgSynMemberInfo
            {
                Size = (ushort)sizeof(MsgSynMemberInfo),
                Id = 1112, Donation = member.GuildDonation, Rank = member.GuildRank,};
            for (byte i = 0; i < member.Name.Length; i++)
                packet.Name[i] = (byte)member.Name[i];
            return packet;
        }


        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgSynMemberInfo*) p;
                    BufferPool.RecycleBuffer(buffer);

                    var name = Encoding.UTF8.GetString(packet.Name, 16);

                    if (GameWorld.Find(name.TrimEnd('\0'), out Player found))
                    {
                        var msg = Create(found);
                        player.Send(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }


        public static implicit operator byte[](MsgSynMemberInfo msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSynMemberInfo*)p = *&msg;
            return buffer;
        }
    }
}