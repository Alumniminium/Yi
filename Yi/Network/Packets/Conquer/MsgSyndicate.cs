using System;
using System.Runtime.InteropServices;
using Yi.Calculations;
using Yi.Database;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Sockets;
using Yi.Structures;
using Yi.World;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgSyndicate
    {
        public ushort Size;
        public ushort Id;
        public GuildRequest Type;
        public int Param;

        public static byte[] Create(int param, GuildRequest type)
        {
            var msg = new MsgSyndicate
            {
                Size = (ushort) sizeof(MsgSyndicate),
                Id = 1107,
                Type = type,
                Param = param
            };
            return msg;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgSyndicate*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Type)
                    {
                        case GuildRequest.ApplyJoin:
                            if (GameWorld.Find(packet.Param, out Player found))
                            {
                                if (found.Guild != null)
                                {
                                    player.JoinGuildRequest = found.UniqueId;
                                    packet.Param = player.UniqueId;
                                    found.Send(packet);
                                }
                            }
                            break;
                        case GuildRequest.InviteJoin:
                            if (GameWorld.Find(packet.Param, out Player found2))
                                if (found2.JoinGuildRequest == player.UniqueId)
                                    player.Guild.Add(found2);
                            break;
                        case GuildRequest.LeaveSyn:
                            player.Guild.Leave(player);
                            break;
                        case GuildRequest.KickOutMember:
                            if (GameWorld.Find(packet.Param, out Player found3))
                                found3.Guild.Leave(found3);
                            break;
                        case GuildRequest.QuerySynName:
                            if (Collections.Guilds.TryGetValue(packet.Param, out var guild))
                                player.Send(MsgName.Create(packet.Param, guild.Name, (byte) MsgNameType.Syndicate));
                            break;
                        //case GuildRequest.SetAlly:
                        //    break;
                        //case GuildRequest.ClearAlly:
                        //    break;
                        //case GuildRequest.SetEnemy:
                        //    break;
                        //case GuildRequest.ClearAntagonize:
                        //    break;
                        case GuildRequest.DonateMoney:
                            if (player.Money >= packet.Param)
                            {
                                player.GuildDonation += packet.Param;
                                player.Guild.Funds += packet.Param;
                                player.Money -= packet.Param;
                                player.Send(packet);
                            }
                            break;
                        case GuildRequest.QuerySynAttr:
                            if (player.UniqueId == packet.Param)
                            {
                                if (player.Guild == null)
                                    return;
                                player.Send(MsgName.Create(player.Guild.UniqueId, player.Guild.Name, (byte) MsgNameType.Syndicate));
                                player.Send(MsgSyndicateSpawn.Create(player));
                                player.Send(MsgText.Create(Constants.System, Constants.Allusers, player.Guild.Bulletin, MsgTextType.GuildBulletin));
                            }
                            break;
                        //case GuildRequest.SetSyn:
                        //    break;
                        //case GuildRequest.UniteSubSyn:
                        //    break;
                        //case GuildRequest.UniteSyn:
                        //    break;
                        //case GuildRequest.SetWhiteSyn:
                        //    break;
                        //case GuildRequest.SetBlackSyn:
                        //    break;
                        //case GuildRequest.DestroySyn:
                        //    break;
                        //case GuildRequest.SetMantle:
                        //    break;
                        default:
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

        public static implicit operator byte[](MsgSyndicate msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSyndicate*)p = *&msg;
            return buffer;
        }
    }
}