using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Discity
{
    [Script(Id = 30100)]
    public class SolarSaint
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(60))
                        {
                            yi
                                .Text(
                                      "Hello my soldier, in order to proceed to the next stage you need to collect 5 SoulStones as a charge for my teleportation service.")
                                .Link("I have collected them.", 1)
                                .Link("That is ok.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.SoulStone, 5))
                        {
                            player.Teleport(230, 330, 2022);
                            //TODO: Add the global message and stageone reward
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Are you trying to fool me? You have NOT collected them yet.")
                                    .Link("I am sorry...", 255)
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