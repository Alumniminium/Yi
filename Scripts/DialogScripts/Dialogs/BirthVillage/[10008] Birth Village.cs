using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.BirthVillage
{
    [Script(Id = 10008)]
    public partial class BirthVillage8
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
                                .Text("Hello world from the first Yi Script!")
                                .Link("Omfg that's soo cool!", 1)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(20))
                        {
                            yi
                                .Text("I know right ?!")
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