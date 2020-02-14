using Yi.Entities;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Scripting;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 35016)]
    public class WuxingOven
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                        player.Send(MsgAction.Create(player, 1, MsgActionType.Dialog));
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}