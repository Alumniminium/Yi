using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 10050)]
    public class Conductress
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(20))
                        {
                            yi
                                .Text("Hello " + player.Name.TrimEnd('\0') + ", I shall charge you 100 gold to teleport you.")
                                .Link("Phoenix Castle", 1)
                                .Link("Ape Mountain", 2)
                                .Link("Desert City", 3)
                                .Link("Bird Island", 4)
                                .Link("Mine Cave", 6)
                                .Link("Market", 5)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(958, 556, 1002);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 2:
                    {
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(555, 957, 1002);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 3:

                    {
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(062, 463, 1002);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 4:
                    {
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(228, 198, 1002);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 5:
                    {
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(212, 196, 1036);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 6:
                        if (player.Money >= 100)
                        {
                            player.Money -= 100;
                            player.Teleport(056, 399, 1002);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, but you do not have enough gold.")
                                    .Link("Alright.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
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