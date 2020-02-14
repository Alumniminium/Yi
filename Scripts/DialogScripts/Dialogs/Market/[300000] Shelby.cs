using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 300000)]
    public class Shelby
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.Level >= 70)
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text(
                                          "We hope all can help each other. If you power level the newbies, we may reward you with Meteors or Dragonballs. Are you interested?")
                                    .Link("Tell me more details.", 1)
                                    .Link("Check my virtue points.", 5)
                                    .Link("Claim prize.", 6)
                                    .Link("Just passing by.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text(
                                          "You must be level 70 at least or more in order to get virtue points and be able to exchange for am prize.")
                                    .Link("I see.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "If you are above level 70 and try to power level the newbies (at least 20 levels lower than you), you may gain virtue points.")
                                .Link("What are the virtue points?", 2)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "The more newbies you power level, the more virtue points you gain. I shall give you a good reward for a certain virtue points.")
                                .Link("How can I gain virtue points?", 3)
                                .Link("What prize can I expect?", 4)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 3:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "Once the newbies are one level up, the team captain can gain virtue points accordingly.")
                                .Link("I see.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 4:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "I shall reward you a Meteor for 5,000 virtue points and a DragonBall for 150.000 virtue points.")
                                .Link("That is ok.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 5:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text("You currently have " + player.VirturePoints +
                                      " virtue points, please try to gain more.")
                                .Link("Great, I will!", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 6:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text("You currently have" + player.VirturePoints +
                                      " virtue points, what prize do you prefer?")
                                .Link("Meteor - 5,000 ", 7)
                                .Link("Meteor - 150.000 ", 8)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 7:
                    {
                        if (player.VirturePoints >= 5000)
                        {
                            player.VirturePoints -= 5000;
                            var item = Item.Factory.Create(ItemNames.Meteor);
                            player.Inventory.AddItem(item);
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("There you go, check your inventory. " + " You currently have" +
                                          player.VirturePoints + "virtue points.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("You do not have enough virtue points, " + " you currently have" +
                                          player.VirturePoints + "virtue points.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 8:
                    {
                        if (player.VirturePoints >= 150000)
                        {
                            player.VirturePoints -= 150000;
                            var item = Item.Factory.Create(ItemNames.Dragonball);
                            player.Inventory.AddItem(item);
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("There you go, check your inventory. " + " You currently have" +
                                          player.VirturePoints + "virtue points.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("You do not have enough virtue points, " + " you currently have" +
                                          player.VirturePoints + "virtue points.")
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