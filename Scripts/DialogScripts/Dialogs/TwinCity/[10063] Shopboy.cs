using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 10063)]
    public class Shopboy
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(7))
                        {
                            yi
                                .Text("Our shop is famous for dyeing, you can have your equipment dyed there, you have a wide choice of colors. One Meteor will be charged before you try the colors. Would you like to try?")
                                .Link("Yes, please.", 1)
                                .Link("I am not interested.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.Meteor))
                        {
                            player.Inventory.RemoveItem(ItemNames.Meteor);
                            player.Teleport(22, 26, 1008);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(7))
                            {
                                yi
                                    .Text("Sorry, you do not have a Meteor.")
                                    .Link("I am sorry.", 255)
                                    .Finish();
                                player.Send(yi);
                            }
                        }
                        break;
                    }
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