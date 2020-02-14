using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 42)]
    public class Warden
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(37))
                        {
                            yi
                                .Text("You can not leave here unless you have paid for your crimes.")
                                .Link("Ok! Let me get out from here!", 1)
                                .Link("I would rather rot in hell!", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.PkPoints >= 30)
                        {
                            using (var yi = new NpcDialog(37))
                            {
                                yi
                                    .Text("You must stay and think about what you've done!")
                                    .Link("I shall..", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            player.Teleport(517, 352, 1002);
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