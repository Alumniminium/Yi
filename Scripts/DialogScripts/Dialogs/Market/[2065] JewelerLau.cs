using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Market
{
    [Script(Id = 2065)]
    public class JewelerLau
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(54))
                        {
                            yi
                                .Text("What kind of gems would you like to compose?")
                                .Link("PhoenixGems.", 1)
                                .Link("DragonGems.", 6)
                                .Link("FuryGems.", 11)
                                .Link("RainbowGems.", 17)
                                .Link("KylinGems.", 22)
                                .Link("VioletGems.", 27)
                                .Link("MoonGems.", 32)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        using (var yi = new NpcDialog(54))
                        {
                            yi
                                .Text("What quality gem would you like your PhoenixGems to be composed into?")
                                .Link("Refined one", 2)
                                .Link("Super one.", 4)
                                .Finish();
                        }
                        break;
                    }
                    case 2:
                    {
                        if (player.Inventory.HasItem(ItemNames.NormalPhoenixGem, 15))
                        {
                            using (var yi = new NpcDialog(54))
                            {
                                yi
                                    .Text("Are you sure that you want to compose your normal PhoenixGems into Refined one?")
                                    .Link("Yes, I am.", 3)
                                    .Link("No, I am not.", 255)
                                    .Finish();
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(54))
                            {
                                yi
                                    .Text("There are no fifteen gems. Please prepare enough gems first.")
                                    .Link("Er, really?", 255)
                                    .Finish();
                            }
                        }
                        break;
                    }
                    case 3:
                    {
                        using (var yi = new NpcDialog(54))
                        {
                            yi
                                .Text("")
                                .Link("", 4)
                                .Finish();
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