using System;
using System.Runtime.InteropServices;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Sockets;
using Yi.SelfContainedSystems;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MsgStorage
    {
        public ushort Length;
        public ushort Id;
        public int UniqueId;
        public MsgStorageAction Action;
        public MsgStorageType Type;
        public ushort Unknown2;
        public int Param;

        public static implicit operator byte[] (MsgStorage msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgStorage*)p = *&msg;
            return buffer;
        }

        public static MsgStorage Create(int uniqueId, MsgStorageAction action)
        {
            var msg = new MsgStorage
            {
                Length = (ushort) sizeof (MsgStorage),
                Id = 1102,
                Param = 0,
                Action = action,
                UniqueId = uniqueId,
                Type = MsgStorageType.Storage,
                Unknown2 = 0,
            };
            return msg;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgStorage*) p;
                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Action)
                    {
                        case MsgStorageAction.Add:
                            AddToStorage(player, ref packet);
                            break;
                        case MsgStorageAction.Remove:
                            RemoveFromStorage(player, ref packet);
                            break;
                        case MsgStorageAction.List:
                            ListStorage(player, ref packet);
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

        private static void ListStorage(Player player, ref MsgStorage packet)
        {
            //player.CurrentStorageId = packet.UniqueId;
            StorageSystem.ShowStock(player);
        }

        private static void AddToStorage(Player player, ref MsgStorage packet)
        {
            var found = player.Inventory.FindByUID(packet.Param);
            if (found.UniqueId == packet.Param)
            {
                if (StorageSystem.PutIn(player, packet.UniqueId, found))
                {
                    player.Send(packet);
                    StorageSystem.ShowStock(player);
                }
            }
        }

        private static void RemoveFromStorage(Player player, ref MsgStorage packet)
        {
            if (StorageSystem.TakeOut(player, packet.UniqueId, packet.Param))
            {
                player.Send(packet);
                StorageSystem.ShowStock(player);
            }
        }
    }
}