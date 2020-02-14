using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 30161)]
    public class Furniture
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
                                .Text("Welcome to Twin City Furniture Store. Currently, you have limited selection but more furniture will come in soon.")
                                .Link("I want to have a look.", 1)
                                .Link("I am not interested.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(42, 42, 1511);
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