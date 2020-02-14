using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Network.Packets.Conquer;
using Yi.Scripting;
using Yi.Structures;
using Yi.World;

namespace DialogScripts.Dialogs.Common
{
    [Script(Id = 1)]
    public class BuySell
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                Console.WriteLine("control:"+control);
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(20))
                        {
                            yi
                                .Text($"Hello {player.Name.TrimEnd('\0')}, do you have anything for me?")
                                .Link("Yes!", 1)
                                .Link("No.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                        Console.WriteLine("Opening Tradewindow");

                        Npc found;
                        if (GameWorld.Find(player.CurrentNpcId, out found))
                        {
                            player.Trade = new Trade(player, found);
                            var packet = MsgTrade.Create(player.CurrentNpcId, TradeMode.ShowTradeWindow);
                            player.ForceSend(packet, packet.Size);
                        }
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}