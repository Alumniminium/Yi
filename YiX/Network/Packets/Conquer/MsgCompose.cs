using System;
using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Enums;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgCompose
    {
        public ushort Size;
        public ushort Id;
        public int UnqiueId;
        public int FirstItemUID;
        public int SecondItemUID;
        public int FirstGemUID;
        public int SecondGemUID;

        public static void Handle(Player player, byte[] buffer)
        {
            fixed (byte* p = buffer)
            {
                var packet = *(MsgCompose*)p;
                BufferPool.RecycleBuffer(buffer);

                try
                {
                    if (!player.Inventory.TryGetItem(packet.UnqiueId, out var mainItem))
                        return;
                    if (!player.Inventory.TryGetItem(packet.FirstItemUID, out var firstTreasure))
                        return;
                    if (!player.Inventory.TryGetItem(packet.SecondItemUID, out var secondTreasure))
                        return;

                    if (firstTreasure.Plus != secondTreasure.Plus)
                        return;

                    if (mainItem.Plus == 0 && firstTreasure.Plus != 1 || mainItem.Plus != 0 && mainItem.Plus != firstTreasure.Plus)
                        return;

                    if (mainItem.Plus >= 9)
                        return;

                    var mainType = (short)(mainItem.ItemId / 1000);
                    var firstTreasureType = (short)(firstTreasure.ItemId / 1000);
                    var secondTreasureType = (short)(secondTreasure.ItemId / 1000);

                    if (firstTreasureType != 730 && secondTreasureType != 730)
                    {
                        if ((short)(mainType / 100) == 4 && ((short)(firstTreasureType / 100) != 4 || (short)(secondTreasureType / 100) != 4))
                            return;

                        if ((short)(mainType / 100) == 5 && ((short)(firstTreasureType / 100) != 5 || (short)(secondTreasureType / 100) != 5))
                            return;

                        if ((short)(mainType / 100) == 9 && ((short)(firstTreasureType / 100) != 9 || (short)(secondTreasureType / 100) != 9))
                            return;

                        if ((short)(mainType / 100) != 4 && (short)(mainType / 100) != 5 && (short)(mainType / 100) != 9)
                            if (mainType != firstTreasureType || mainType != secondTreasureType)
                                return;
                    }

                    if (mainItem.Plus >= 5)
                    {
                        if (!player.Inventory.TryGetItem(packet.FirstGemUID, out var firstGem))
                            return;
                        if (!player.Inventory.TryGetItem(packet.SecondGemUID, out var secondGem))
                            return;

                        player.Inventory.RemoveItem(firstGem);
                        player.Inventory.RemoveItem(secondGem);
                    }

                    player.Inventory.RemoveItem(firstTreasure);
                    player.Inventory.RemoveItem(secondTreasure);

                    mainItem.Plus++;
                    player.Send(new MsgItemInformation(mainItem, MsgItemInfoAction.Update, MsgItemPosition.Inventory));
                }
                catch (Exception exc)
                {
                    Output.WriteLine(exc);
                }
            }
        }
    }
}