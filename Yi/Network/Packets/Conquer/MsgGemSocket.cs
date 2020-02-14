using System;
using System.Runtime.InteropServices;
using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.Network.Sockets;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgGemSocket
    {
        public ushort Size;
        public ushort Id;
        public int UnqiueId;
        public int ItemUID;
        public int GemUID;
        public ushort SocketID;
        public ushort RemoveGem;

        public static byte[] Create(int uid, int itemuid, int gemuid, ushort socketid, ushort removegem)
        {
            var msgP = stackalloc MsgGemSocket[1];
            {
                msgP->Size = (ushort) sizeof(MsgGemSocket);
                msgP->Id = 1027;
                msgP->UnqiueId = uid;
                msgP->ItemUID = itemuid;
                msgP->GemUID = gemuid;
                msgP->SocketID = socketid;
                msgP->RemoveGem = removegem;
            }
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgGemSocket*)p = *msgP;
            return buffer;
        }


        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgGemSocket*) p;
                    BufferPool.RecycleBuffer(buffer);

                    player.Inventory.Items.TryGetValue(packet.ItemUID, out var item); //get the item now

                    if (player.UniqueId != packet.UnqiueId)
                    {
                        Output.WriteLine("MsgGemSocket player not equal to packet's player uid.");
                        return;
                    }
                    if (item == null)
                    {
                        Output.WriteLine("MsgGemSocket player's item is null.");
                        return;
                    }

                    Output.WriteLine("Testing MsgGemSocket.. " + packet.RemoveGem);

                    switch (packet.RemoveGem)
                    {
                        case 0:
                        {
                            player.Inventory.Items.TryGetValue(packet.GemUID, out var gem);

                            if (gem == null)
                            {
                                Output.WriteLine("MsgGemSocket player's gem is null.");
                                return;
                            }

                            if (packet.SocketID == 1)
                                item.Gem1 = (byte) (gem.ItemId % 100);
                            else
                                item.Gem2 = (byte) (gem.ItemId % 100);

                            player.Inventory.RemoveItem(item);
                            player.Inventory.AddOrUpdate(item.UniqueId, item); //we could just addorupdate etc after the switch?
                            player.Inventory.RemoveItem(gem);

                            player.Send(new MsgItemInformation(item, MsgItemPosition.Inventory));
                            break;
                        }
                        case 1:
                        {
                            if (packet.SocketID == 1)
                                item.Gem1 = 255;
                            else
                                item.Gem2 = 255;

                            player.Inventory.RemoveItem(item);
                            player.Inventory.AddOrUpdate(item.UniqueId, item); //we could just addorupdate etc after the switch?

                            player.Send(new MsgItemInformation(item, MsgItemPosition.Inventory));
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }
    }
}