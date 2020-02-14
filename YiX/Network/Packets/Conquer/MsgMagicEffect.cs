using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgMagicEffect
    {
        public ushort Size;
        public ushort Id;
        public int UniqId;
        public int Param; //TargetUID || (X, Y)
        public ushort Type;
        public short Level;
        public int TargetCount;
        public fixed int Targets [180];

        public static IEnumerable<byte[]> Create(YiObj attacker, IEnumerable<(YiObj, int)> targetEnumerable)
        {
            const int maxTargets = 60;
            var targets = targetEnumerable.ToDictionary(pair => pair.Item1, pair => pair.Item2);
            var packetCount = (int)Math.Max(1, Math.Ceiling((float)targets.Count / maxTargets));
            var packets = new byte[packetCount][];
            var entities = targets.Keys.ToArray();
            var damages = targets.Values.ToArray();
            for (var i = 0; i < packetCount; i++)
            {
                var packet = stackalloc MsgMagicEffect[1];
                {
                    packet->Size = (ushort)(28 + 12 * Math.Min(Math.Min(targets.Count - i * maxTargets, maxTargets),targets.Count));
                    packet->Id = 1105;
                    packet->UniqId = attacker.UniqueId;
                    packet->Param = (int)attacker.Direction;
                    packet->Type = attacker.CurrentSkill.Id;
                    packet->Level = attacker.CurrentSkill.Level;
                    packet->TargetCount = Math.Min(Math.Min(targets.Count - i * maxTargets, maxTargets), targets.Count);
                };
                var offset = 0;
                for (var j = 0; j < Math.Min(targets.Count - i * maxTargets, maxTargets); j++)
                {
                    packet->Targets[offset++] = entities[j + i * maxTargets].UniqueId;
                    packet->Targets[offset++] = damages[j + i * maxTargets];
                    packet->Targets[offset++] = 0;
                }
                var buffer = BufferPool.GetBuffer();
                fixed (byte* p = buffer)
                    *(MsgMagicEffect*)p = *packet;
                packets[i] = buffer;
            }
            return packets;
        }

        public static byte[] Create(YiObj attacker, YiObj target, int damage)
        {
            var packet = stackalloc MsgMagicEffect[1];
            {
                packet->Size = 40;
                packet->Id = 1105;
                packet->UniqId = attacker.UniqueId;
                packet->Param = (int) attacker.Direction;
                packet->Type = attacker.CurrentSkill.Id;
                packet->Level = attacker.CurrentSkill.Level;
                packet->TargetCount = 1;
            };
            packet->Targets[0] = target.UniqueId;
            packet->Targets[1] = damage;
            packet->Targets[2] = 0;


            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgMagicEffect*)p = *packet;

            return buffer;
        }

        public static implicit operator byte[](MsgMagicEffect msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgMagicEffect*)p = *&msg;
            return buffer;
        }
    }
}