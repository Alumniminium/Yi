using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 6003)]
    public class PrisonOfficer
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
                                .Text("Would you like to visit the BotJail?")
                                .Link("Yes, please.", 1)
                                .Link("No, thank you.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.PkPoints <= 30)
                            player.Teleport(29, 71, 6001);
                        else
                        {
                            using (var yi = new NpcDialog(32))
                            {
                                yi
                                    .Text("Sorry, your PkPoints are above 30, so that I can not let you visit the BotJail.")
                                    .Link("No, thank you.", 255)
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