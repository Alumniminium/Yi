using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Sockets;
using YiX.World;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgFriend
    {
        public ushort Size;
        public ushort Id;
        public int UniqId;
        public MsgFriendActionType Action;
        public MsgFriendStatusType IsOnline;
        public long Unknow1;
        public short Unknow2;
        public fixed byte Name [16];

        public static byte[] Create(Player target, MsgFriendActionType action, MsgFriendStatusType status)
        {
            var packet = new MsgFriend
            {
                Size = (ushort) sizeof(MsgFriend),
                Id = 1019,
                Action = action,
                IsOnline =status,
                Unknow1 = 0,
                Unknow2 = 0,
                UniqId = target.UniqueId,
            };
            for (byte i = 0; i < target.Name.Length; i++)
                packet.Name[i] = (byte)target.Name[i];
            return packet;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgFriend*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Action)
                    {
                        case MsgFriendActionType.None:
                            break;
                        case MsgFriendActionType.FriendApply:
                            FriendApply(player, ref packet);
                            break;
                        case MsgFriendActionType.FriendBreak:
                            BreakFrienship(player, ref packet);
                            break;
                        case MsgFriendActionType.GetInfo:
                            GetInfo(player, ref packet);
                            break;
                        case MsgFriendActionType.EnemyOnline:
                            break;
                        case MsgFriendActionType.EnemyOffline:
                            break;
                        case MsgFriendActionType.EnemyDel:
                            break;
                        case MsgFriendActionType.EnemyAdd:
                            break;

                        default:
                            Output.WriteLine($"MsgFriend Subtype not implemented: {Enum.GetName(typeof(MsgFriendActionType), packet.Action)}");
                            Output.WriteLine(((byte[]) packet).HexDump());
                            break;

                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }
        private static void GetInfo(Player player, ref MsgFriend packet)
        {
            
        }
        private static void BreakFrienship(Player player, ref MsgFriend packet)
        {
            if (player.Friends.Contains(packet.UniqId))
                player.Friends.Remove(packet.UniqId);

            if (GameWorld.Find(packet.UniqId, out Player found))
            {
                if (found.Friends.Contains(player.UniqueId))
                    found.Friends.Remove(player.UniqueId);

                found.Send(Create(player,MsgFriendActionType.FriendBreak, MsgFriendStatusType.Offline));
            }
            player.Send(Create(found, MsgFriendActionType.FriendBreak, MsgFriendStatusType.Offline));
        }
        private static void FriendApply(Player player, ref MsgFriend packet)
        {
            if (GameWorld.Find(packet.UniqId, out Player target))
            {
                if (target.FriendRequestTarget == player.UniqueId)
                {
                    player.ForceSend(BufferPool.Clone(packet), packet.Size);
                    target.ForceSend(packet, packet.Size);
                    player.Send(Create(target, MsgFriendActionType.FriendAccept, MsgFriendStatusType.Online));
                    target.Send(Create(player, MsgFriendActionType.FriendAccept, MsgFriendStatusType.Online));

                    if (player.Friends == null)
                        player.Friends = new List<int>();
                    if (target.Friends == null)
                        target.Friends = new List<int>();

                    if (!player.Friends.Contains(target.UniqueId))
                        player.Friends.Add(target.UniqueId);
                    if (!target.Friends.Contains(player.UniqueId))
                        target.Friends.Add(player.UniqueId);
                }
                else
                {
                    player.FriendRequestTarget = target.UniqueId;
                    target.ForceSend(packet, packet.Size);
                }
            }
        }


        public static implicit operator byte[](MsgFriend msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgFriend*)p = *&msg;
            return buffer;
        }
    }
}