using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.ApeMountain
{
    [Script(Id = 100379)]
    public class SpaceMark39
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.IsWaterTaoist())
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Hello " + player.Name.TrimEnd('\0') +
                                          ", I am here to sell you a WindSpell for 100 silver, are you interested?")
                                    .Link("Yes, I am.", 1)
                                    .Link("No, I am not.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.IsWaterTaoist() && player.Money >= 100)
                        {
                            player.Money -= 100;
                            var windSpell = Item.Factory.Create(ItemNames.SpaceMark3);
                            player.Inventory.AddItem(windSpell);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Sorry, you do not have enough silver.")
                                    .Link("That is ok.", 255)
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