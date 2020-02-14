using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 4293)]
    public class MineAdmin
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
                                .Text("Hello, I am the assistant of the mine union. If you want to enter the mine cave, I can send you.")
                                .Link("Yes, please.", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(155, 94, 1028);
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