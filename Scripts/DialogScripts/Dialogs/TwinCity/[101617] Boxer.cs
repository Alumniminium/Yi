using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 101617)]
    public class Boxer
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
                                .Text("If you are level 20 or above, you may level up at the training grounds. it does not consume any durability, life and mana. Do you want me to teleport you there?")
                                .Link("Please teleport me there.", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Money >= 1000)
                        {
                            player.Money -= 1000;
                            player.Teleport(215, 217, 1039);
                        }
                        else
                        {
                            using (var yi = new NpcDialog(20))
                            {
                                yi
                                    .Text("Sorry, you do not have enough gold.")
                                    .Link("I see.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
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