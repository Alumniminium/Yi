using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.MeteorZone
{
    [Script(Id = 300653)]
    public class Grandpa
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(79))
                        {
                            yi
                                .Text("Would you like to get teleported to StoneCity?")
                                .Link("Yes, please", 1)
                                .Link("No, thanks.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(435, 337, 1077);
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