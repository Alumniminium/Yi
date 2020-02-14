using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Labyrinth
{
    [Script(Id = 1153)]
    public class EastGeneral
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(32))
                        {
                            yi
                                .Text("Do you want to get teleported to the next floor?")
                                .Link("Yes, please.", 1)
                                .Link("No, thank you.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.SkyToken))
                        {
                            player.Inventory.RemoveItem(ItemNames.SkyToken);
                            player.Teleport(104, 108, 1352);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(32))
                            {
                                yi
                                    .Text("Sorry, you do not have a SkyToken.")
                                    .Link("I am sorry.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
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