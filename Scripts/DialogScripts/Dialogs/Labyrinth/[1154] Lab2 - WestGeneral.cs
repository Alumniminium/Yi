using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Labyrinth
{
    [Script(Id = 1154)]
    public class WestGeneral
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
                        if (player.Inventory.HasItem(ItemNames.EarthToken))
                        {
                            player.Inventory.RemoveItem(ItemNames.EarthToken);
                            player.Teleport(139, 147, 1353);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(32))
                            {
                                yi
                                    .Text("Sorry, you do not have an EarthToken.")
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