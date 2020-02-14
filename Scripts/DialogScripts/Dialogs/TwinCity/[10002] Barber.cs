using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.TwinCity
{
    [Script(Id = 10002)]
    public class Barber
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text("I can help you find a new hairstyle just for you! Only 500 silver to re-invent yourself.")
                                .Link("New styles", 1)
                                .Link("Nostalgic styles", 3)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        if (player.Money >= 500)
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Please choose the style you want!")
                                    .Link("Style 01", 30)
                                    .Link("Style 02", 31)
                                    .Link("Style 03", 32)
                                    .Link("Style 04", 33)
                                    .Link("Style 05", 34)
                                    .Link("Style 06", 35)
                                    .Link("Next Page", 2)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Sorry, you do not have enough silver.")
                                    .Link("I see.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }

                        break;
                    }
                    case 2:
                    {
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text("Please choose the style you want!")
                                .Link("Style 01", 36)
                                .Link("Style 02", 37)
                                .Link("Style 03", 38)
                                .Link("Style 04", 39)
                                .Link("Style 05", 40)
                                .Link("Style 06", 41)
                                .Link("Previous Page", 1)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 3:
                    {
                        if (player.Money >= 500)
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Please choose the style you want!")
                                    .Link("Style 01", 10)
                                    .Link("Style 02", 11)
                                    .Link("Style 03", 12)
                                    .Link("Style 04", 13)
                                    .Link("Style 05", 14)
                                    .Link("Style 06", 15)
                                    .Link("Style 07", 16)
                                    .Link("Style 08", 17)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(1))
                            {
                                yi
                                    .Text("Sorry, you do not have enough silver.")
                                    .Link("I see.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    {
                        player.Hair = (ushort)(player.Hair / 100 * 100 + control);
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text("Do you like this style?")
                                .Link("Yes, I do!", 4)
                                .Link("No, I would like to change it.", 3)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    {
                        player.Hair = (ushort)(player.Hair / 100 * 100 + control);
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text("Do you like this style?")
                                .Link("Yes, I do!", 4)
                                .Link("No, I would like to change it.", 1)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 4:
                    {
                        player.Money -= 500;
                        using (var yi = new NpcDialog(1))
                        {
                            yi
                                .Text(
                                      "Perfection! I hope you enjoy your new haircut. Come back any time if you wish to change it again.")
                                .Link("Will do, thanks!", 255)
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