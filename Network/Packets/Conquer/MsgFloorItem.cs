using System;
using System.Runtime.InteropServices;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgFloorItem
    {
        public ushort Size;
        public ushort Id;
        public int UniqueId, ItemId;
        public ushort X, Y;
        public MsgFloorItemType MsgFloorItemType;

        public static byte[] Create(FloorItem item, MsgFloorItemType type)
        {
            var packet = new MsgFloorItem
            {
                Size = (ushort)sizeof(MsgFloorItem),
                Id = 1101,
                UniqueId = item.UniqueId,
                X = item.Location.X,
                Y = item.Location.Y,
                ItemId = item.Original.Valid() ? (int)item.Look : item.Original.ItemId,
                MsgFloorItemType = type,
            };
            return packet;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgFloorItem*)p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.MsgFloorItemType)
                    {
                        case MsgFloorItemType.Pick:
                            PickupItem(player, ref packet);
                            break;
                        case MsgFloorItemType.DisplayEffect:
                            break;
                        case MsgFloorItemType.SynchroTrap:
                            break;
                        case MsgFloorItemType.RemoveEffect:
                            break;
                        default:
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

        private static void PickupItem(Player player, ref MsgFloorItem packet) => FloorItemSystem.Pickup(player, packet.UniqueId);

        public static implicit operator byte[](MsgFloorItem msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgFloorItem*)p = *&msg;
            return buffer;
        }
    }
}