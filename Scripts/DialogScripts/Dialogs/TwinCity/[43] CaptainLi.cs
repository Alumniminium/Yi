using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 43)]
    public class CaptainLi
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
                                .Text("What can I do for you?")
                                .Link("Visit the jail.", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Money >= 1000)
                        {
                            player.Money -= 1000;
                            player.Teleport(29, 72, 6000);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(37))
                            {
                                yi
                                    .Text("Sorry, you do not have 1000 silvers.")
                                    .Link("I see.", 255)
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