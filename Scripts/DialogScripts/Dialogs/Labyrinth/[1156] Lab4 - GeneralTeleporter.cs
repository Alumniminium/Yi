using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Labyrinth
{
    [Script(Id = 1156)]
    public class GeneralTeleporter
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
                                .Text("Do you want to get teleported to TwinCity?")
                                .Link("Yes, please.", 1)
                                .Link("No, thank you.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(400, 400, 1002);
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