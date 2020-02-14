using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.ApeMountain
{
    [Script(Id = 422)]
    public class OldQuarrier
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.Inventory.HasItem(ItemNames.EuxeniteOre, 10))
                        {
                            using (var yi = new NpcDialog(14))
                            {
                                yi
                                    .Text(
                                          "Since you take 10 EuxeniteOres. I will refine Saltpeter for you.")
                                    .Link("Yes, please.", 1)
                                    .Link("No, thanks.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(14))
                            {
                                yi
                                    .Text(
                                          "Norbert recommened me to you, did he?")
                                    .Link("Yes, I come for Saltpeter..", 2)
                                    .Link("No, just passing by", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Inventory.RemoveItem(ItemNames.EuxeniteOre, 10);
                        var item = Item.Factory.Create(ItemNames.Salpeter);
                        player.Inventory.AddItem(item);
                        using (var yi = new NpcDialog(14))
                        {
                            yi
                                .Text(
                                      "My pleasure. Here you are.")
                                .Link("Thank you.", 255)
                                .Finish();

                            player.Send(yi);
                        }

                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(14))
                        {
                            yi
                                .Text(
                                      "Let's make long story short. 10 EuxeniteOres are required for a piece of Saltpeter. You can dig EuxeniteOres in the plain mine.")
                                .Link("I will get some.", 255)
                                .Finish();

                            player.Send(yi);
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