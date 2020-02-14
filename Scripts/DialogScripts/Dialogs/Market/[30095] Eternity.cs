using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 30095)]
    public class Eternity
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (!player.Reborn)
                        {
                            //   if(Player.Level >=120 && Player.ProfessionLevel == 5 || (Player.Class == 135 && Player.Level >= 110))
                            using (var yi = new NpcDialog(35))
                            {
                                yi
                                    .Text("Modify me.")
                                    .Link("Link me.", 1)
                                    .Link("Fuck me.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(35))
                            {
                                yi
                                    .Text("You are already a reborned character, I can not help you anymore.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }

                    case 1:
                    {
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