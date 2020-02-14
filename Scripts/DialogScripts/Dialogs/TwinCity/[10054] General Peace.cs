using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 10054)]
    public class GeneralPeace
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(7))
                        {
                            yi
                                .Text("This is the way to the Desert City. Although you are excellent, it is dangerous to go ahead.")
                                .Link("I want to go.", 1)
                                .Link("I think I will stay here.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(971, 666, 1000);
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