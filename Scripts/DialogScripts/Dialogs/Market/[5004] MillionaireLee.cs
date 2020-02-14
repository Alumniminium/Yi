using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 5004)]
    public class MillionaireLee
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "I can pack Meteors and DragonBalls for you. Add me 10 Meteors or DragonBalls, I will make it to a MeteorScroll or DragonBallScroll that occupies only one slot.")
                                .Link("Cool. Pack my Meteors.", 1)
                                .Link("Cool. Pack my DragonBalls", 2)
                                .Finish();

                            player.Send(yi);
                        }

                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.Meteor, 10))
                        {
                            player.Inventory.RemoveItem(ItemNames.Meteor, 10);
                            var item = Item.Factory.Create(ItemNames.MeteorScroll);
                            player.Inventory.AddItem(item);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Sorry, you do not have 10 Meteors.")
                                    .Link("Ok wait, I will check.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 2:
                    {
                        if (player.Inventory.HasItem(ItemNames.Dragonball, 10))
                        {
                            player.Inventory.RemoveItem(ItemNames.Dragonball, 10);
                            var item = Item.Factory.Create(ItemNames.DragonballScroll);
                            player.Inventory.AddItem(item);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Sorry, you do not have 10 DragonBalls.")
                                    .Link("Ok wait, I will check.", 255)
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