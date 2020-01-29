using System;
using System.Runtime.InteropServices;
using YiX.AttackSystems;
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
    public unsafe struct MsgInteract
    {
        public ushort Size;
        public ushort Id;
        public int Timestamp;
        public int AttackerUniqueId;
        public int TargetUniqueId;
        public ushort X;
        public ushort Y;
        public MsgInteractType Type;
        public int Value;

        public static byte[] Create(YiObj source, YiObj target, MsgInteractType type, int value)
        {
            if (target == null||source==null) return null;
            var msgP = stackalloc MsgInteract[1];
            msgP->Size = (ushort) sizeof(MsgInteract);
            msgP->Id = 1022;
            msgP->Timestamp = Environment.TickCount;
            msgP->AttackerUniqueId = source.UniqueId;
            msgP->TargetUniqueId = target.UniqueId;
            msgP->X = target.Location.X;
            msgP->Y = target.Location.Y;
            msgP->Type = type;
            msgP->Value = value;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgInteract*)p = *msgP;
            return buffer;
        }
        public static byte[] Create(int attackerUniqueId, int targetUniqueId, ushort targetX, ushort targetY, MsgInteractType type, int value)
        {
            var msgP = stackalloc MsgInteract[1];
            msgP->Size = (ushort)sizeof(MsgInteract);
            msgP->Id = 1022;
            msgP->Timestamp = Environment.TickCount;
            msgP->AttackerUniqueId = attackerUniqueId;
            msgP->TargetUniqueId = targetUniqueId;
            msgP->X = targetX;
            msgP->Y = targetY;
            msgP->Type = type;
            msgP->Value = value;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgInteract*)p = *msgP;
            return buffer;
        }
        public static byte[] Die(YiObj attacker, YiObj target)
        {
            var msgP = stackalloc MsgInteract[1];
            msgP->Size = 32;
            msgP->Id = 1022;
            msgP->Timestamp = Environment.TickCount;
            msgP->AttackerUniqueId = attacker.UniqueId;
            msgP->TargetUniqueId = target.UniqueId;
            msgP->X = target.Location.X;
            msgP->Y = target.Location.Y;
            msgP->Type = MsgInteractType.Death;
            msgP->Value = 0;
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgInteract*)p = *msgP;
            return buffer;
        }


        public static void Handle(Player player, byte[] buffer)
        {
            if (buffer == null) return;
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgInteract*) p;
                    BufferPool.RecycleBuffer(buffer);

                    //Output.WriteLine("Attack Packet from: "+packet.AttackerUniqueId + " of type: "+packet.Type);

                    if (packet.Type == MsgInteractType.Magic)
                    {
                        Crypto.DecryptSkill(player, ref packet, out var skill);
                        player.CurrentSkill = skill;
                    }

                    if (packet.AttackerUniqueId != player.UniqueId)
                    {
                        if (GameWorld.Find(packet.TargetUniqueId, out YiObj found))
                        {
                            packet.Value = 1000;
                            player.Send(packet);
                            found.GetHit(player, 1000);
                        }
                        return;
                    }

                    switch (packet.Type)
                    {
                        case MsgInteractType.Archer:
                            PhysicalAttack(player, ref packet);
                            break;
                        case MsgInteractType.Physical:
                            PhysicalAttack(player, ref packet);
                            break;
                        case MsgInteractType.Magic:
                            MagicAttack(player,ref packet);
                            break;
                        case MsgInteractType.MonsterHunter:
                            player.Send(packet);
                            break;
                        default:
                            player.Send(packet);
                            Output.WriteLine($"Unknown InteractionType: {packet.Type}");
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

        private static void MagicAttack(Player player, ref MsgInteract packet)
        {
            player.Equipment.RemoveDura(MsgItemPosition.RightWeapon);
            if (player.HasFlag(StatusEffect.Invisibility))
                player.RemoveStatusEffect(StatusEffect.Invisibility);
            if (player.HasFlag(StatusEffect.SpawnProtection))
                player.RemoveStatusEffect(StatusEffect.SpawnProtection);

            if (GameWorld.Maps[player.MapId].Find(packet.TargetUniqueId, out YiObj target))
            {
                if (packet.TargetUniqueId != 0)
                    player.CurrentTarget = target;
            }

            Magic.ExecuteAttack(player, new Vector2(packet.X, packet.Y));
        }

        private static void PhysicalAttack(YiObj attacker, ref MsgInteract packet)
        {
            if (attacker.HasFlag(StatusEffect.Invisibility))
                attacker.RemoveStatusEffect(StatusEffect.Invisibility);
            if (attacker.HasFlag(StatusEffect.SpawnProtection))
                attacker.RemoveStatusEffect(StatusEffect.SpawnProtection);

            if (!GameWorld.Maps[attacker.MapId].Find(packet.TargetUniqueId, out YiObj currentTarget))
                return;

            attacker.CurrentTarget = currentTarget;
            packet.Value = Physical.PhysicalAttack(attacker, packet.Type);

            attacker.Equipment.RemoveDura(MsgItemPosition.LeftWeapon);
            attacker.Equipment.RemoveDura(MsgItemPosition.RightWeapon);
            attacker.Equipment.RemoveDura(MsgItemPosition.Ring);
            attacker.AddWeaponProf(MsgItemPosition.LeftWeapon, (uint)packet.Value);
            attacker.AddWeaponProf(MsgItemPosition.RightWeapon, (uint)packet.Value);

            if (packet.Type == MsgInteractType.Archer)
                packet.Value = packet.Value / 2;

            if (packet.Value == -1)
                return;
            
            if (attacker.ActivatePassiveSkill(attacker, ref packet.Value, out var skill)&& skill.HasValue)
            {
                attacker.CurrentSkill = skill.Value;
                Magic.ExecuteAttack((Player) attacker, attacker.Location);
                return;
            }

            ScreenSystem.Send(attacker, packet, true);
            currentTarget.GetHit(attacker, packet.Type == MsgInteractType.Archer? packet.Value * 2: packet.Value);
        }

        public static implicit operator byte[](MsgInteract msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgInteract*)p = *&msg;
            return buffer;
        }
    }
}