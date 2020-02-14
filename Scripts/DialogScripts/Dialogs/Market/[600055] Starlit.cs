using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 600055)]
    public class Starlit
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text("Modify me.")
                                .Link("Link me.", 1)
                                .Link("Fuck me.", 255)
                                .Finish();

                            player.Send(yi);
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