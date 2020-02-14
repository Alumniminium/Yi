using System;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Moonbox
{
    [Script(Id = 600003)]
    public class Maggie
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
                                .Text("I am the entrance to the tactics, do you want to get teleported there?")
                                .Link("Definitely!", 1)
                                .Link("Just passing by.", 255)
                                .Finish();

                            player.Send(yi);
                        }
                        break;
                    }
                    case 1:
                    {
                        var random = new Random();
                        var map = random.Next(1, 8);
                        switch (map)
                        {
                            case 1:
                            {
                                player.Teleport(215, 160, 1043);
                                break;
                            }
                            case 2:
                            {
                                player.Teleport(215, 160, 1044);
                                break;
                            }
                            case 3:
                            {
                                player.Teleport(215, 160, 1045);
                                break;
                            }
                            case 4:
                            {
                                player.Teleport(215, 160, 1046);
                                break;
                            }
                            case 5:
                            {
                                player.Teleport(215, 160, 1047);
                                break;
                            }
                            case 6:
                            {
                                player.Teleport(215, 160, 1048);
                                break;
                            }
                            case 7:
                            case 8:
                            {
                                player.Teleport(215, 160, 1049);
                                break;
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