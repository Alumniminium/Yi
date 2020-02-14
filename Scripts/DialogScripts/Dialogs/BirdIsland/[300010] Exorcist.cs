using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.BirdIsland
{
    [Script(Id = 300010)]
    public class Exorcist
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
                                .Text("The Ancient Devil was sealed in this island. The seal's power is very weak now. The Devil is gonna wake up. Can you help us?")
                                .Link("How can I help you?", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(32))
                        {
                            yi
                                .Text("First, get 5 Amulets. Each amulet is protected by a Guard of different profession. Only if you are of the same profession, you can challenge the Guard.")
                                .Text(" So you had better ask your friend for help. After you gather the 5 Amulets, click on the yellow marks on the ground to bring out the devil and ")
                                .Text("it is guards. Enable PK mode to kill them. Will you help us?")
                                .Link("Yes, I shall try.", 2)
                                .Link("Let me think it over.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 2:
                    {
                        player.Teleport(188, 232, 1082);
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