using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 600050)]
    public class Fortuneteller
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(17))
                        {
                            yi
                                .Text("Have you heard of Palace Method?")
                                .Link("Palace Method?", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(17))
                        {
                            yi
                                .Text("Are you interested? I would like to tell you more and hope you can work it out.")
                                .Link("Yea, please go ahead.", 2)
                                .Link("Sorry, I am very busy.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(17))
                        {
                            yi
                                .Text(
                                      "I discovered a mystic tactic a few days ago. I tried to work it out, but I failed and almost died from it.")
                                .Link("I would like to have a try.", 3)
                                .Link("Too dangerous.", 255)
                                .Finish();

                            player.Send(yi);
                        }

                        break;
                    }
                    case 3:
                    {
                        using (var yi = new NpcDialog(17))
                        {
                            yi
                                .Text(
                                      "It is very dangerous. Are you sure you'll try? If yes, I shall teleport you there. Talk to Maggie to enter the tactics.")
                                .Link("Yeah.", 4)
                                .Link("I changed my mind.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 4:
                    {
                        if (player.Level >= 70)
                        {
                            player.Teleport(026, 027, 1042);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(17))
                            {
                                yi
                                    .Text(
                                          "Sorry, you must be level 70 or above.")
                                    .Link("Ok, sorry.", 255)
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