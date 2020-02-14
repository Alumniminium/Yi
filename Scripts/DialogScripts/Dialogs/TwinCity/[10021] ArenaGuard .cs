using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 10021)]
    public class Arena

    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(20))
                        {
                            yi
                                .Text("Hello " + player.Name.TrimEnd('\0') +
                                      ", would you like to enter the PK arena? Be careful you there are a lot of PKers!")
                                .Link("Yes, please.", 1)
                                .Link("Teleport me to the infinity stamina arena!", 2)
                                .Link("No, thanks.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(51, 71, 1005);
                        break;
                    }
                    case 2:
                    {
                        // Player.Teleport(51, 71, 1005);
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