using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 3381)]
    public class SurgeonMiracle
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
                                      "Hello, are you satisfield with your stature? I can change your body size for 1 DragonBall.")
                                .Link("Yes, please.", 1)
                                .Link("No, I am satisfied with my look.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Inventory.HasItem(ItemNames.Dragonball))
                        {
                            player.Inventory.RemoveItem(ItemNames.Dragonball);
                            switch (player.Look % 10000)
                            {
                                case 1001:
                                {
                                    player.Look = 1002;
                                    break;
                                }
                                case 1002:
                                {
                                    player.Look = 1001;
                                    break;
                                }
                                case 1003:
                                {
                                    player.Look = 1004;
                                    break;
                                }
                                case 1004:
                                {
                                    player.Look = 1003;
                                    break;
                                }
                                case 2001:
                                {
                                    player.Look = 2002;
                                    break;
                                }
                                case 2002:
                                {
                                    player.Look = 2001;
                                    break;
                                }
                                case 2003:
                                {
                                    player.Look = 2004;
                                    break;
                                }
                                case 2004:
                                {
                                    player.Look = 2003;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Sorry, you do not have a DragonBall.")
                                    .Link("That is ok.", 255)
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