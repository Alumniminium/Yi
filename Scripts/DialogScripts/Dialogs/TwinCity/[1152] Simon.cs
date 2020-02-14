using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 1152)]
    public class Simon
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(40))
                        {
                            yi
                                .Text("Great rewards will attract many brave people. I am looking for brave people to help me take my patrimony back. Can you help? The rewards are handsome.")
                                .Link("Please tell me more.", 1)
                                .Link("I got SunDiamonds.", 7)
                                .Link("I got MoonDiamonds.", 9)
                                .Link("I got StarDiamonds.", 11)
                                .Link("I got CloudDiamonds.", 13)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(40))
                        {
                            yi
                                .Text("My ancestors built a Labyrinth long before. Many treasures were stored there like SunDiamonds, MoonDiamonds, Start Diamonds and so on. But it was occupied by fiece monsters soon. They expelled our clansmen and kept the treasures.")
                                .Link("It is a pity", 2)
                                .Link("I have no interest.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(40))
                        {
                            yi
                                .Text("I have always been here waiting for brave people to help me. Of course I can not trust in those who do not have 2000 Virtue points.")
                                .Link("How about me?", 3)
                                .Link("Sorry. That is too tough for me.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 3:
                    {
                        if (player.VirturePoints >= 2000)
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("It seems that you have the 2000 Virtue, do you want me to teleport you?")
                                    .Link("Yes, please", 4)
                                    .Link("I am not interested.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Sorry, you do not have enough Virtue Points")
                                    .Link("Sigh...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 4:
                    {
                        player.VirturePoints -= 2000;
                        player.Teleport(016, 128, 1351);
                        break;
                    }
                    case 5:
                    {
                        using (var yi = new NpcDialog(40))
                        {
                            yi
                                .Text("SunDiamond, MoonDiamonds, StartDiamond and CloudDimaond are kept by different monsters. If you get them for me, I will give you some rewards.")
                                .Link("What rewards?", 6)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 6:
                    {
                        using (var yi = new NpcDialog(40))
                        {
                            yi
                                .Text("xxx for 17 SunDiamonds, xx for 17 MoonDiamonds, xx for 17 StarDiamonds and an AncestorBox for 17 CloudDiamonds. If you are lucky enough, you will get a big surprise from the box.")
                                .Link("I see. Thank you.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 7:
                    {
                        if (player.Inventory.HasItem(ItemNames.SunDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text(
                                          "You have the required amount of SunDiamonds, would you like to receive your reward?")
                                    .Link("Yes, please.", 8)
                                    .Link("No, thanks.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text(
                                          "You gotta be kidding me. How dare you come here to claim prize with so few SunDiamonds?")
                                    .Link("Wait. I will get more!", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 8:
                    {
                        if (player.Inventory.HasItem(ItemNames.SunDiamond, 17))
                        {
                            player.Inventory.RemoveItem(ItemNames.SunDiamond, 17);
                            var item = Item.Factory.Create(ItemNames.Meteor);
                            player.Inventory.AddItem(item, 5);
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Congratulations! Check your inventory to see the awesome reward!")
                                    .Link("Thank you.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.SunDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 9:
                    {
                        if (player.Inventory.HasItem(ItemNames.MoonDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text(
                                          "You have the required amount of MoonDiamonds, would you like to receive your reward?")
                                    .Link("Yes, please.", 10)
                                    .Link("No, thanks.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.MoonDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 10:
                    {
                        if (player.Inventory.HasItem(ItemNames.MoonDiamond, 17))
                        {
                            player.Inventory.RemoveItem(ItemNames.MoonDiamond, 17);
                            var item = Item.Factory.Create(ItemNames.Meteor);
                            player.Inventory.AddItem(item, 7);
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Congratulations! Check your inventory to see the awesome reward!")
                                    .Link("Thank you.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.MoonDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 11:
                    {
                        if (player.Inventory.HasItem(ItemNames.StarDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text(
                                          "You have the required amount of StarDiamonds, would you like to receive your reward?")
                                    .Link("Yes, please", 12)
                                    .Link("No, thanks", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.StarDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 12:
                    {
                        if (player.Inventory.HasItem(ItemNames.StarDiamond, 17))
                        {
                            player.Inventory.RemoveItem(ItemNames.StarDiamond, 17);
                            var item = Item.Factory.Create(ItemNames.Panacea);
                            //  var random = new System.Random();
                            // var gemType = (byte) random.Next(0, 7);
                            /*   item += (gemType*10) + 1;
                                    if(item != 700000)*/
                            player.Inventory.AddItem(item);
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Congratulations! Check your inventory to see the awesome reward!")
                                    .Link("Thank you.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.StarDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 13:
                    {
                        if (player.Inventory.HasItem(ItemNames.CloudDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text(
                                          "You have the required amount of CloudDiamonds, would you like to receive your reward?")
                                    .Link("Yes, please", 14)
                                    .Link("No, thanks", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.CloudDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }

                    case 14:
                    {
                        if (player.Inventory.HasItem(ItemNames.CloudDiamond, 17))
                        {
                            player.Inventory.RemoveItem(ItemNames.CloudDiamond, 17);
                            var item = Item.Factory.Create(ItemNames.Salpeter);
                            player.Inventory.AddItem(item);
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Congratulations! Check your inventory to see the awesome reward!")
                                    .Link("Thank you.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Inventory.Count > 35 && player.Inventory.HasItem(ItemNames.CloudDiamond, 17))
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("You need more space in inventory to be able to receive the reward.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(40))
                            {
                                yi
                                    .Text("Are you trying to fool me? You do not have enough Diamonds!")
                                    .Link("I am sorry...", 255)
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