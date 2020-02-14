using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 350050)]
    public class CelestialTao
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.Reborn && player.Level >= 70)
                        {
                            using (var yi = new NpcDialog(22))
                            {
                                yi
                                    .Text(
                                          "The DragonBall is really a precious item.You can use it to reallot your attribute points after you have been reborned.")
                                    .Text(
                                          "After reborning at 70 or above you can reallot your attribute points freely. Well, if you have a DragonBall, I can help you do that.")
                                    .Link("Please help me.", 1)
                                    .Link("Definitely.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (!player.Reborn)
                        {
                            using (var yi = new NpcDialog(22))
                            {
                                yi
                                    .Text("Please come back when you have been reborned at least once in your lifetime.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else if (player.Level < 70)
                        {
                            using (var yi = new NpcDialog(22))
                            {
                                yi
                                    .Text("You must be above level 70 to be able to reallot your attribute points.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(22))
                            {
                                yi
                                    .Text(
                                          "You need to be reborn and above level 70 to be able to reallot your attribute points.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.Dragonball))
                        {
                            player.Inventory.RemoveItem(ItemNames.Dragonball);
                            var playerAttributePoints =
                                (ushort)(player.Vitality + player.Strength + player.Spirit + player.Agility +
                                         player.Statpoints);
                            player.Vitality = player.Strength = player.Agility = player.Spirit = 0;
                            player.Statpoints = playerAttributePoints;
                        }
                        else
                        {
                            using (var yi = new NpcDialog(22))
                            {
                                yi
                                    .Text("Sorry, but you must pay one DragonBall come back when you have one!")
                                    .Link("That is ok.", 255)
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