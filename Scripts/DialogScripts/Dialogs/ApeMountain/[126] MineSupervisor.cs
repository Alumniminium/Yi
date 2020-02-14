using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.ApeMountain
{
    [Script(Id = 126)]
    public class MineSupervisor
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(9))
                        {
                            yi
                                .Text("Hello, I am the entrance to the Ape Mountain mine. Would you like to be teleported there?")
                                .Link("Yes, please.", 1)
                                .Link("No, thanks.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Level >= 40)
                            player.Teleport(137, 102, 1026);
                        else
                        {
                            using (var yi = new NpcDialog(9))
                            {
                                yi
                                    .Text("Sorry, you must be at least level 40 to be able to enter this mine.")
                                    .Link("Sorry...", 255)
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