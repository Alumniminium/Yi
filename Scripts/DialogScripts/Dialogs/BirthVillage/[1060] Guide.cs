using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.BirthVillage
{
    [Script(Id = 1060)]
    public class Guide
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(22))
                        {
                            yi
                                .Text("Hello " + player.Name.TrimEnd('\0') + ", Welcome to Yi!")
                                .Text("Please select a Face - free of charge - from the guy next to me")
                                .Link("Will do!", 1)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(22))
                        {
                            yi
                                .Text("Great! See you around!")
                                .Link("Yea! See you later!", 255)
                                .Finish();

                            player.Send(yi);
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