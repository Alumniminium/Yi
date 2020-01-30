using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using YiX.Calculations;
using YiX.Database.Squiggly;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgAction
    {
        public ushort Size;
        public ushort Id;
        public int Timestamp;
        public int UniqueId;
        public int Param;
        public ushort X;
        public ushort Y;
        public ushort Direction;
        public MsgActionType Type;

        public static byte[] Pathfinding(Player player)
        {
            byte[] pack = new MsgAction
            {
                Size = (ushort)sizeof(MsgAction),
                Id = 1010,
                Timestamp = Environment.TickCount,
                UniqueId = player.UniqueId,
                Param = (player.Location.X + 1 << 16) | (player.Location.Y & 0xffff),
                Type = MsgActionType.Pathfinding
            };
            return pack;
        }
        public static byte[] Create(int timestamp, int uniqueId, int param, ushort x, ushort y, ushort direction, MsgActionType type)
        {
            MsgAction* msgP = stackalloc MsgAction[1];
            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = timestamp;
            msgP->UniqueId = uniqueId;
            msgP->Param = param;
            msgP->X = x;
            msgP->Y = y;
            msgP->Direction = direction;
            msgP->Type = type;

            var buffer = BufferPool.GetBuffer();
            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));
            return buffer;
        }
        public static byte[] Create(YiObj player, int param, MsgActionType type)
        {
            MsgAction* msgP = stackalloc MsgAction[1];
            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = Environment.TickCount;
            msgP->UniqueId = player.UniqueId;
            msgP->Param = param;
            msgP->Type = type;
            var buffer = BufferPool.GetBuffer();
            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));
            return buffer;
        }
        public static byte[] Create2(YiObj player, int param, MsgActionType type)
        {
            MsgAction* msgP = stackalloc MsgAction[1];
            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = Environment.TickCount;
            msgP->UniqueId = player.UniqueId;
            msgP->Param = param;
            msgP->Type = type;
            var buffer = BufferPool.GetBuffer();
            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));
            return buffer;
        }
        public static byte[] LevelUp(YiObj obj) => new MsgAction { Size = (ushort)sizeof(MsgAction), Id = 1010, Timestamp = Environment.TickCount, UniqueId = obj.UniqueId, Param = 0, X = obj.Location.X, Y = obj.Location.Y, Direction = (byte)obj.Direction, Type = MsgActionType.Leveled };
        public static byte[] Spawn(YiObj obj) => new MsgAction { Size = (ushort)sizeof(MsgAction), Id = 1010, Timestamp = Environment.TickCount, UniqueId = obj.UniqueId, Param = 0, X = obj.Location.X, Y = obj.Location.Y, Direction = (byte)obj.Direction, Type = MsgActionType.EntitySpawn };
        public static byte[] SpawnEffect(YiObj obj)
        {
            MsgAction* msgP = stackalloc MsgAction[1];
            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = Environment.TickCount;
            msgP->UniqueId = obj.UniqueId;
            msgP->X = obj.Location.X;
            msgP->Y = obj.Location.Y;
            msgP->Direction = (byte)obj.Direction;
            msgP->Type = MsgActionType.SpawnEffect;
            var buffer = BufferPool.GetBuffer();
            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));
            return buffer;
        }
        public static byte[] MapShowPacket(YiObj obj)
        {
            MsgAction* msgP = stackalloc MsgAction[1];
            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = Environment.TickCount;
            msgP->UniqueId = obj.UniqueId;
            msgP->Param = obj.MapId;
            msgP->X = obj.Location.X;
            msgP->Y = obj.Location.Y;
            msgP->Direction = (byte)obj.Direction;
            msgP->Type = MsgActionType.MapShow;
            var buffer = BufferPool.GetBuffer();

            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));

            return buffer;
        }
        public static byte[] CashEffect(YiObj obj, int amount) => new MsgAction { Size = (ushort)sizeof(MsgAction), Id = 1010, Timestamp = Environment.TickCount, UniqueId = obj.UniqueId, Param = amount, X = obj.Location.X, Y = obj.Location.Y, Direction = (byte)obj.Direction, Type = MsgActionType.PickupCashEffect };

        public static byte[] Jump(YiObj obj, int x, int y)
        {
            MsgAction* msgP = stackalloc MsgAction[1];

            msgP->Size = (ushort)sizeof(MsgAction);
            msgP->Id = 1010;
            msgP->Timestamp = Environment.TickCount;
            msgP->UniqueId = obj.UniqueId;
            msgP->Param = y << 16 | x & 0xffff;
            msgP->Direction = (byte)obj.Direction;
            msgP->Type = MsgActionType.Jump;

            var buffer = BufferPool.GetBuffer();
            Marshal.Copy((IntPtr)msgP, buffer, 0, sizeof(MsgAction));
            return buffer;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgAction*)p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Type)
                    {
                        case MsgActionType.MapShow:
                            MapShow(player, ref packet);
                            break;

                        case MsgActionType.QueryTeamMemberPos:
                            if (TeamSystem.Teams.ContainsKey(packet.Param))
                            {
                                var target = TeamSystem.Teams[packet.Param].Members[packet.Param];
                                player.Send(Create(Environment.TickCount, target.UniqueId, target.UniqueId, target.Location.X, target.Location.Y, 0, MsgActionType.QueryTeamMemberPos));
                            }
                            break;

                        case MsgActionType.EndFly:
                            EndFly(player, ref packet);
                            break;
                        case MsgActionType.Portal:
                            PortalEnter(player, ref packet);
                            break;
                        case MsgActionType.StartVending:
                            OpenShop(player, packet);
                            break;
                        case MsgActionType.ConfirmFriends:
                            ConfirmFriends(player, ref packet);
                            break;
                        case MsgActionType.Hotkeys:
                        case MsgActionType.StopVending:
                        case MsgActionType.ConfirmGuild:
                        case MsgActionType.ConfirmProf:
                        case MsgActionType.ConfirmSkills:
                        case MsgActionType.OnTeleport:
                        case MsgActionType.FinishTeleport:
                            player.Send(packet);
                            break;

                        case MsgActionType.DropMagic:
                            DropMagic(player, ref packet);
                            break;
                        case MsgActionType.DropSkill:
                            break;

                        case MsgActionType.Action:
                            DoEmote(player, ref packet);
                            break;

                        case MsgActionType.Jump:
                            Jump(player, ref packet);
                            break;
                        case MsgActionType.PetJump:
                            PetJump(player, ref packet);
                            break;

                        case MsgActionType.ChangeDirection:
                            ChangeDirection(player, ref packet);
                            break;

                        case MsgActionType.ChangePkMode:
                            ChangePkMode(player, ref packet);
                            break;

                        case MsgActionType.Revive:
                            Revive(player);
                            break;

                        case MsgActionType.Mine:
                            Mine(player, ref packet);
                            break;

                        case MsgActionType.ChangeFace:
                            ChangeFace(player, ref packet);
                            break;

                        case MsgActionType.EntitySpawn:
                            SpawnEntity(player, ref packet);
                            break;

                        case MsgActionType.QueryFriendEquip:
                        case MsgActionType.ViewOthersEquip:
                            ViewEquip(player, ref packet);
                            break;
                        case MsgActionType.QueryEnemyInfo:
                        case MsgActionType.QueryFriendInfo:
                            QueryFriendInfo(player, ref packet);
                            break;

                        case MsgActionType.EndXpList:
                            EndXPList(player, ref packet);
                            break;
                        case MsgActionType.EndTransform:
                            EndTransform(player, ref packet);
                            break;

                        case MsgActionType.DeleteChar:
                            DeleteChar(player, ref packet);
                            break;
                        default:
                            Output.WriteLine($"MsgAction Subtype not implemented: {Enum.GetName(typeof(MsgActionType), packet.Type)}");
                            Output.WriteLine(((byte[])packet).HexDump());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        private static void DropMagic(Player s, ref MsgAction packet)
        {
            Output.WriteLine("IT IS CALLING IT");
            s.Skills.Remove((SkillId)packet.Param);
            s.Send(packet);
        }

        private static void DeleteChar(Player player, ref MsgAction packet)
        {
            if (player.UniqueId != packet.UniqueId) return;

            SelectorSystem.RemoveCharacter(player);
            player.Disconnect();
        }
        private static void EndTransform(Player player, ref MsgAction packet)
        {
            player.DelTransform();
            EntityLogic.Recalculate(player);
            player.Send(packet);
        }
        private static void EndXPList(Player player, ref MsgAction packet)
        {
            player.Xp = 0;
            BuffSystem.Clear(player);
            player.RemoveStatusEffect(StatusEffect.XpList);
            player.RemoveStatusEffect(StatusEffect.Cyclone);
            player.RemoveStatusEffect(StatusEffect.SuperMan);
            player.RemoveStatusEffect(StatusEffect.XpShield);

            player.Send(packet);

            if (player.HasFlag(StatusEffect.Die))
                player.AddStatusEffect(StatusEffect.Die);
        }
        private static void EndFly(Player player, ref MsgAction packet)
        {
            player.RemoveStatusEffect(StatusEffect.Flying);
            player.Send(packet);
        }
        private static void ConfirmFriends(Player player, ref MsgAction packet)
        {
            player.Send(packet);
            if (!player.Online)
                return;

            if (player.Friends == null)
                player.Friends = new List<int>();

            if (player.Enemies == null)
                player.Enemies = new List<int>();

            foreach (var friend in player.Friends)
            {
                if (GameWorld.Find(friend, out Player found))
                    found.Send(MsgFriend.Create(player, MsgFriendActionType.FriendOnline, MsgFriendStatusType.Online));
                player.Send(MsgFriend.Create(found, MsgFriendActionType.GetInfo, found.Online ? MsgFriendStatusType.Online : MsgFriendStatusType.Offline));
            }
            foreach (var enemy in player.Enemies)
            {
                if (GameWorld.Find(enemy, out Player found))
                    player.Send(MsgFriend.Create(found, MsgFriendActionType.EnemyAdd, found.Online ? MsgFriendStatusType.Online : MsgFriendStatusType.Offline));
            }
        }
        private static void QueryFriendInfo(Player player, ref MsgAction packet)
        {
            if (GameWorld.Find(packet.Param, out Player target))
                player.Send(MsgFriendInfo.Create(target));
        }
        private static void ViewEquip(Player player, ref MsgAction packet)
        {
            if (!GameWorld.Find(packet.Param, out Player found))
                return;

            foreach (var item in found.Equipment.Items)
            {
                var itemInfo = new MsgItemInformation(found, item.Value, item.Key);
                player.ForceSend(itemInfo, itemInfo.Size);
            }
        }

        private static void SpawnEntity(Player player, ref MsgAction packet)
        {
            if (!GameWorld.Find(packet.Param, out YiObj found))
                return;
            player.Send(packet);

            switch (found)
            {
                case Monster monster:
                    player.Send(MsgSpawn.Create(monster)); break;
                case Player playerFound:
                    player.Send(MsgSpawn.Create(playerFound)); break;
            }

        }

        private static void OpenShop(Player player, MsgAction packet)
        {
            player.BoothId = 10000000 + player.UniqueId;
            player.Direction = (Direction)packet.Direction;
            BoothSystem.CreateFor(player);
            packet.Param = player.BoothId;
            player.Send(packet);
        }

        private static void ChangeDirection(YiObj player, ref MsgAction packet)
        {
            player.Direction = (Direction)packet.Direction;
            ScreenSystem.Send(player, packet, true);
        }

        private static void ChangeFace(YiObj player, ref MsgAction packet)
        {
            player.Look = (uint)(player.Look - (int)(player.Look / 10000) * 10000 + packet.Param * 10000);
            if (TeamSystem.Teams.ContainsKey(player.UniqueId))
            {
                foreach (var member in TeamSystem.Teams[player.UniqueId].Members)
                    (member.Value as Player)?.Send(MsgTeamUpdate.JoinLeave(player, MsgTeamMemberAction.AddMember));
            }
            ScreenSystem.Send(player, packet, true);
        }

        private static void ChangePkMode(Player player, ref MsgAction packet)
        {
            player.PkMode = (PkMode)packet.Param;
            player.Send(packet);
        }
        private static void DoEmote(YiObj player, ref MsgAction packet)
        {
            player.Emote = (Emote)packet.X;
            ScreenSystem.Send(player, packet, true);
        }
        private static void Jump(Player player, ref MsgAction packet)
        {
            player.CurrentTarget = null;
            player.RemoveSpawnProtection();
            //TODO Distance & Speed Checks

            if (GameWorld.Maps.ContainsKey(player.MapId))
            {
                if (GameWorld.Maps[player.MapId].GroundValid((ushort)packet.Param, (ushort)(packet.Param >> 16)))
                {
                    if (!ScreenSystem.SendJump(player, ref packet))
                        ScreenSystem.Send(player, packet, false, true, packet.Size);
                    player.Direction = Position.GetDirection(player.Location.X, player.Location.Y, (ushort)packet.Param, (ushort)(packet.Param >> 16));
                    player.Location.X = (ushort)packet.Param;
                    player.Location.Y = (ushort)(packet.Param >> 16);
                    if (TeamSystem.Teams.ContainsKey(player.UniqueId))
                        TeamSystem.Teams[player.UniqueId].UpdateLeaderPosition(player);
                    ScreenSystem.Update(player);
                    //Message.SendTo(player, $"Loc: X={player.Location.X} Y={player.Location.Y}", MsgTextType.Action);
                    return;
                }
            }
            else
            {
                player.GetMessage("SERVER", player.Name, "THIS DMAP IS NOT LOADED!" + player.MapId, MsgTextType.Center);
            }
            player.Teleport(player.Location.X, player.Location.Y, player.MapId);
            ScreenSystem.Update(player);
        }

        private static void PetJump(Player player, ref MsgAction packet)
        {
            var r = Position.GetDirection(player.Location.X, player.Location.Y, (ushort)packet.Param, (ushort)(packet.Param >> 16));
            var x = (ushort)packet.Param;
            var y = (ushort)(packet.Param >> 16);

            if (player.Pet != null)
            {
                player.Pet.Location.X = x;
                player.Pet.Location.Y = y;
                player.Pet.Direction = r;

                if (!ScreenSystem.SendJump(player, ref packet))
                    ScreenSystem.Send(player, packet, false, true, packet.Size);
                ScreenSystem.Update(player);
            }
        }

        private static void MapShow(Player player, ref MsgAction packet)
        {
            player.ForceSend(MapShowPacket(player), (ushort)sizeof(MsgAction));
            player.ForceSend(MsgStatus.Create(player.MapId, 0), (ushort)sizeof(MsgStatus));

            if (player.Inventory.Items == null)
                player.Inventory = new Inventory(player);
            else
                player.Inventory.SetOwner(player);
            if (player.Equipment.Items == null)
                player.Equipment = new Equipment(player);
            else
                player.Equipment.SetOwner(player);
            foreach (var kvp in player.Equipment)
                player.Send(new MsgItemInformation(kvp.Value, kvp.Key));

            foreach (var kvp in player.Inventory)
                player.Send(new MsgItemInformation(kvp.Value, MsgItemPosition.Inventory));

            foreach (var kvp in player.Profs)
                player.Send(MsgProf.Create(kvp.Value));

            foreach (var kvp in player.Skills)
                player.Send(MsgSkill.Create(kvp.Value));

            EntityLogic.Recalculate(player);

            if (player.Online)
                return;
            player.AddStatusEffect(StatusEffect.Frozen);
            player.CurrentNpcId = 1337;
            player.Send(LegacyPackets.NpcSay("Select Char."));
            player.Send(LegacyPackets.NpcLink("Next", 1));
            player.Send(LegacyPackets.NpcLink("Select", 10));
            player.Send(LegacyPackets.NpcLink("New", 100));
            player.Send(LegacyPackets.NpcFace(10));
            player.Send(LegacyPackets.NpcFinish());

            ScreenSystem.Create(player);
            ScreenSystem.Update(player);

            ScreenSystem.Send(player, Create(player, player.UniqueId, MsgActionType.SpawnEffect), true);
        }

        private static void Mine(YiObj player, ref MsgAction packet)
        {
            player.Mining = true;
            ScreenSystem.Send(player, packet, true);
        }

        private static void PortalEnter(YiObj player, ref MsgAction packet)
        {
            var portalX = (ushort)packet.Param;
            var portalY = (ushort)(packet.Param >> 16);

            if (player.UniqueId != packet.UniqueId)
                return;

            if (GameWorld.Maps.ContainsKey(player.MapId))
            {
                foreach (var portal in GameWorld.Maps[player.MapId].Portals.Values.Where(portal => portal.X == portalX && portal.Y == portalY))
                {
                    var passway = PortalProcessor.FindPortal((int)portal.IdX, player.MapId);
                    player.Teleport((ushort)passway.portal_x, (ushort)passway.portal_y, (ushort)passway.mapid);
                    return;
                }
            }

            Output.WriteLine($"Portal from {portalX}-{portalY} on {player.MapId} not found.");
            player.Teleport((ushort)(player.Location.X - 3), (ushort)(player.Location.Y - 3), player.MapId);
        }

        private static void Revive(Player player)
        {
            player.AddSpawnProtection();

            if (GameWorld.Maps.TryGetValue(player.MapId, out var map) && map.RespawnLocation != null)
                player.Teleport(map.RespawnLocation.Item1, map.RespawnLocation.Item1, map.RespawnLocation.Item3);
            else
                player.Teleport(438, 377, 1002);

            player.CurrentHp = player.MaximumHp;
            player.CurrentMp = player.MaximumMp;
            player.RemoveStatusEffect(StatusEffect.Die);
            player.DelTransform();
        }

        public static implicit operator byte[](MsgAction msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgAction*)p = *&msg;
            return buffer;
        }
    }
}