using System;
using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using YiX.World;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgTeam
    {
        public ushort Size;
        public ushort Id;
        public MsgTeamAction Mode;
        public int TargetUniqueId;

        public static byte[] Create(YiObj human, MsgTeamAction action)
        {
            var msg = new MsgTeam
            {
                Size = (ushort)sizeof(MsgTeam),
                Id = 1023, Mode = action, TargetUniqueId = human.UniqueId};
            return msg;
        }

        public static byte[] CreateTeam(YiObj human) => Create(human, MsgTeamAction.Create);
        public static byte[] DisbandTeam(YiObj human) => Create(human, MsgTeamAction.Dismiss);
        public static byte[] Kick(YiObj human) => Create(human, MsgTeamAction.Kick);
        public static byte[] Invite(YiObj human) => Create(human, MsgTeamAction.Invite);
        public static byte[] Leave(YiObj human) => Create(human, MsgTeamAction.LeaveTeam);

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgTeam*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Mode)
                    {
                        case MsgTeamAction.Create:
                            CreateTeam(player, ref packet);
                            break;
                        case MsgTeamAction.RequestJoin:
                            RequestJoin(player, ref packet);
                            break;
                        case MsgTeamAction.Invite:
                            Invite(player, ref packet);
                            break;
                        case MsgTeamAction.AcceptInvite:
                            AcceptInvite(player, ref packet);
                            break;
                        case MsgTeamAction.AcceptJoin:
                            AcceptJoin(player, ref packet);
                            break;
                        case MsgTeamAction.Dismiss:
                            DismissTeam(player, ref packet);
                            break;
                        case MsgTeamAction.LeaveTeam:
                            Leave(player, ref packet);
                            break;
                        case MsgTeamAction.Kick:
                            Kick(player, ref packet);
                            break;
                        case MsgTeamAction.ForbidNewMembers:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].Locked = true;
                            break;
                        case MsgTeamAction.AllowNewMembers:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].Locked = false;
                            break;
                        case MsgTeamAction.ForbidMoney:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].MoneyLocked = true;
                            break;
                        case MsgTeamAction.AllowMoney:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].MoneyLocked = false;
                            break;
                        case MsgTeamAction.ForbidItems:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].ItemsLocked = true;
                            break;
                        case MsgTeamAction.AllowItems:
                            if (TeamSystem.Teams[player.UniqueId].Leader.UniqueId != packet.TargetUniqueId)
                                break;
                            TeamSystem.Teams[player.UniqueId].ItemsLocked = false;
                            break;
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

        private static void Leave(YiObj player, ref MsgTeam packet)
        {
            if (!TeamSystem.Teams.ContainsKey(player.UniqueId))
                return;
            if (!TeamSystem.Teams[player.UniqueId].Members.ContainsKey(packet.TargetUniqueId))
                return;
            if (!TeamSystem.Teams[player.UniqueId].Members.TryGetValue(packet.TargetUniqueId, out var leader))
                return;
            TeamSystem.Leave(TeamSystem.Teams[player.UniqueId].Leader, player);
        }

        private static void AcceptInvite(YiObj player, ref MsgTeam packet)
        {
            if (GameWorld.Maps[player.MapId].Entities.TryGetValue(packet.TargetUniqueId, out var found))
            {
                var leader = found;
                TeamSystem.Join(leader, player);
            }
        }

        private static void Invite(YiObj player, ref MsgTeam packet)
        {
            if (!TeamSystem.Teams.ContainsKey(player.UniqueId))
                return;
            if (TeamSystem.Teams[player.UniqueId].Members.ContainsKey(packet.TargetUniqueId))
                return;
            if (TeamSystem.Teams[player.UniqueId].Members.Count > 4)
                return;
            
            if (!GameWorld.Maps[player.MapId].Entities.TryGetValue(packet.TargetUniqueId, out var found))
                return;
            if (found == null)
                return;
            var target = found;
            (target as Player)?.Send(Invite(player));
            if (target is Bot)
                TeamSystem.Join(player, target);
        }

        private static void Kick(YiObj player, ref MsgTeam packet)
        {
            if (!TeamSystem.Teams.ContainsKey(player.UniqueId))
                return;
            if (!TeamSystem.Teams[player.UniqueId].Members.ContainsKey(packet.TargetUniqueId))
                return;
            if (!TeamSystem.Teams[player.UniqueId].Members.TryGetValue(packet.TargetUniqueId, out var kick))
                return;
            TeamSystem.Leave(player, kick);
        }

        private static void DismissTeam(YiObj leader, ref MsgTeam packet)
        {
            if (packet.TargetUniqueId != leader.UniqueId)
                return;
            if (!TeamSystem.Teams.ContainsKey(leader.UniqueId))
                return;
            if (leader.UniqueId != TeamSystem.Teams[leader.UniqueId].Leader.UniqueId)
                return;
            TeamSystem.Disband(leader);
        }

        private static void AcceptJoin(YiObj player, ref MsgTeam packet)
        {
            if (!TeamSystem.Teams.ContainsKey(player.UniqueId))
                return;
            if (TeamSystem.Teams[player.UniqueId].Members.Count > 4)
                return;
            if (!GameWorld.Maps[player.MapId].Entities.TryGetValue(packet.TargetUniqueId, out var found))
                return;
            if (found == null)
                return;
            TeamSystem.Join(player, found);
        }

        private static void CreateTeam(Player player, ref MsgTeam packet)
        {
            if (GameWorld.Maps.TryGetValue(player.MapId, out var map))
            {
                if (map.Flags.HasFlag(MapFlags.DisableTeams))
                {
                    player.GetMessage("SYSTEM",player.Name,"Team's are not allowed on this map.",MsgTextType.Action);
                    return;
                }
            }

            TeamSystem.Teams.AddOrUpdate(player.UniqueId, new TeamSystem.TeamData(player));
            player.Send(packet);
        }

        private static void RequestJoin(YiObj player, ref MsgTeam packet)
        {
            if (GameWorld.Maps[player.MapId].Entities.TryGetValue(packet.TargetUniqueId, out var obj))
            {
                if (obj == null)
                    return;

                var leader = obj;
                if (!TeamSystem.Teams.ContainsKey(leader.UniqueId))
                    return;
                if (TeamSystem.Teams[leader.UniqueId].Members.Count > 4)
                    return;
                if (TeamSystem.Teams[leader.UniqueId].Locked)
                {
                    Message.SendTo(player, "TeamData currently not accepting new members.", MsgTextType.Top);
                    return;
                }
                if (leader is Bot)
                {
                    //leader.TeamData.Join(player);
                    return;
                }
                var playerLeader = leader as Player;
                if (playerLeader != null)
                {
                    packet.TargetUniqueId = player.UniqueId;
                    playerLeader.Send(packet);
                }
            }
        }

        public static unsafe implicit operator byte[](MsgTeam msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgTeam*)p = *&msg;
            return buffer;
        }
    }
}