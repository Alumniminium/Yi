using Yi.Enums;
using Yi.Helpers;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Moonbox
{
    [Script(Id = 600005)]
    public class Ghost
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.Inventory.HasItem(ItemNames.CommandToken))
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Would you like to leave this tactic?")
                                    .Link("Yes, please.", 1)
                                    .Link("No, thanks.", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        else
                        {
                            using (var yi = new NpcDialog(60))
                            {
                                yi
                                    .Text("Sorry, you do not have the token from this tactic.")
                                    .Link("I am sorry...", 255)
                                    .Finish();

                                player.Send(yi);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        player.Teleport(025, 025, 1042);
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

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.CommandToken2))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Would you like to leave this tactic?")
        //                            .Link("Yes, please.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                Player.Teleport(025, 025, 1042);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.CommandToken3))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Would you like to leave this tactic?")
        //                            .Link("Yes, please.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                Player.Teleport(025, 025, 1042);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.CommandToken4))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Would you like to leave this tactic?")
        //                            .Link("Yes, please.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                Player.Teleport(025, 025, 1042);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.CommandToken5))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Would you like to leave this tactic?")
        //                            .Link("Yes, please.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                Player.Teleport(025, 025, 1042);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.CommandToken6))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Would you like to leave this tactic?")
        //                            .Link("Yes, please.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                Player.Teleport(025, 025, 1042);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItems(ItemNames.CommandToken, ItemNames.CommandToken2,
        //                    ItemNames.CommandToken3
        //                    , ItemNames.CommandToken4, ItemNames.CommandToken5, ItemNames.CommandToken6))
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text(
        //                                "Since you have the 7 tokens, I can send you to the life tactic, would you like to get teleported there?")
        //                            .Link("Yes, I do.", 1)
        //                            .Link("No, thanks.", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                else
        //                {
        //                    using (var Yi = new NpcDialog(60))
        //                    {
        //                        Yi
        //                            .Text("Sorry, you do not have the token from this tactic.")
        //                            .Link("I am sorry...", 255)
        //                            .Finish();

        //                        Player.Send(Yi);
        //                    }
        //                }
        //                break;
        //            }
        //            case 1:
        //            {
        //                player.Inventory.RemoveItems(ItemNames.CommandToken, ItemNames.CommandToken2,
        //                    ItemNames.CommandToken3
        //                    , ItemNames.CommandToken4, ItemNames.CommandToken5, ItemNames.CommandToken6);
        //                var item = Item.CreateFor(ItemNames.SoulJade);
        //                player.Inventory.AddItem(item);
        //                Player.Teleport(215, 160, 1050);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}