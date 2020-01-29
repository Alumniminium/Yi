using System.Runtime.InteropServices;
using YiX.Database;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using YiX.World;
using Npc = YiX.Entities.Npc;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 101)]
    public unsafe struct MsgNpcSpawn
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId;
        public ushort X;
        public ushort Y;
        public ushort Look;
        public ushort Type;
        public ushort Sort;
        public ushort Base;

        public static byte[] Create(Npc npc)
        {
            var packet = new MsgNpcSpawn
            {
                Size = (ushort)sizeof(MsgNpcSpawn),
                Id = 2030,
                UniqueId = npc.UniqueId,
                Look = (ushort)npc.Look,
                X = npc.Location.X,
                Y = npc.Location.Y,
                Type = npc.Type,
                Sort = npc.Sort,
                Base = npc.Base
            };
            return packet;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            fixed (byte* p = buffer)
            {
                var packet = *(MsgNpcSpawn*) p;
                BufferPool.RecycleBuffer(buffer);

                var npc = new Npc
                {
                    UniqueId = UniqueIdGenerator.GetNext(EntityType.DynamicNpc),
                    MapId = player.MapId,
                    Location =
                    {
                        X = packet.X,
                        Y = packet.Y
                    },
                    Look = packet.Look,
                    Type = 2,
                    Direction = (Direction) (packet.Look % 10)
                };
                GameWorld.Maps[player.MapId].LoadInEntity(npc);
                ScreenSystem.Create(npc);
                ScreenSystem.SendSpawn(npc);
                Collections.Npcs.Add(npc.UniqueId, npc);
            }
        }

        public static implicit operator byte[](MsgNpcSpawn msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgNpcSpawn*)p = *&msg;
            return buffer;
        }
    }
}