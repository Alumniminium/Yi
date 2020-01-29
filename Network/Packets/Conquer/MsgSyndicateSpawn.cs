using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;
using YiX.World;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgSyndicateSpawn
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public int Donation;
        public int Funds;
        public int MemberCount;
        public GuildRanks Rank;
        public fixed byte LeaderName [16];

        public static byte[] Create(YiObj human)
        {
            MsgSyndicateSpawn* msgP = stackalloc MsgSyndicateSpawn[1];

            msgP->Size = (ushort)sizeof(MsgSyndicateSpawn);
            msgP->Id = 1106;
            if (human.Guild != null)
            {
                msgP->UniqueId = human.Guild.UniqueId;
                msgP->Donation = human.GuildDonation;
                msgP->Funds = human.Guild.Funds;
                msgP->Rank = human.GuildRank;
                msgP->MemberCount = human.Guild.Members.Count;
                if (GameWorld.Find(human.Guild.Leader, out Player leader))
                {
                    for (byte i = 0; i < leader.Name.Length; i++)
                        msgP->LeaderName[i] = (byte)leader.Name[i];
                }
            }
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSyndicateSpawn*)p = *msgP;
            return buffer;
        }

        public static implicit operator byte[] (MsgSyndicateSpawn msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSyndicateSpawn*)p = *&msg;
            return buffer;
        }

        //public static byte[] Create(YiObj human)
        //{
        //    var msg = new MsgSyndicateSpawn {Size = 34, Id = 1106, UniqueId = human.Guild.UniqueId, Donation = human.GuildDonation, Funds = human.Guild.Funds, Rank = human.GuildRank, MemberCount = human.Guild.Members.Count};
        //    for (byte i = 0; i < human.Guild.Leader.Name.Length; i++)
        //        msg.LeaderName[i] = (byte)human.Guild.Leader.Name[i];
        //    return msg;
        //}

        //public static implicit operator byte[](MsgSyndicateSpawn msg)
        //{
        //    var buffer = BufferPool.GetBuffer();
        //    fixed (byte* p = buffer)
        //        *(MsgSyndicateSpawn*)p = *&msg;
        //    return buffer;
        //}
    }
}