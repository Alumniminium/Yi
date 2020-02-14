using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.ApeMountain
{
    [Script(Id = 10053)]
    public class Contuctress
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text("Where are you heading for? I can teleport you anywhere for 100 silver.")
                                .Link("Twin City.", 1)
                                .Link("Market", 2)
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
                            player.Teleport(383, 016, 1020);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Sorry, you do not have enough money.")
                                    .Link("That is ok.", 255)
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
                            player.Teleport(212, 196, 1036);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Sorry, you do not have enough money.")
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