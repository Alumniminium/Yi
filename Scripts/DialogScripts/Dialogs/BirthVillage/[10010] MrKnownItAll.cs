using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.BirthVillage
{
    [Script(Id = 10010)]
    public class MrKnownItAll
    {
        public bool ExecuteDialog(Player player, uint npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                        {
                            player.Teleport(438, 377, 1002);
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