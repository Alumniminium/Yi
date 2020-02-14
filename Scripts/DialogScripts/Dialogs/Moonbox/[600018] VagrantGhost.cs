using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Player = Yi.Entities.Player;

namespace DialogScripts.Dialogs.Moonbox
{
    [Script(Id = 600018)]
    public class VagrantGhost
    {
        public static bool Execute(Player player, int npcId, byte control, string input)
        {
            try
            {
                switch (control)
                {
                    case 0:
                    {
                        if (player.Inventory.RemoveItem(ItemNames.SoulJade))
                        {
                            using (var banzai = new NpcDialog(20))
                            {
                                banzai
                                    .Text(
                                          "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
                                    .Link("Oh! Thanks a lot!.", 255)
                                    .Finish();

                                player.Send(banzai);
                            }
                            var moonBox = Item.Factory.Create(ItemNames.Moonbox);
                            player.Inventory.AddItem(moonBox);
                            player.Teleport(025, 025, 1042);
                        }
                        else
                        {
                            using (var banzai = new NpcDialog(20))
                            {
                                banzai
                                    .Text("Sorry, you do not have a SoulJade.")
                                    .Link("That is ok.", 255)
                                    .Finish();

                                player.Send(banzai);
                            }
                            player.Teleport(025, 025, 1042);
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

        //public static bool Execute(Player player, int npcId, byte control, string input)
        //{
        //    try
        //    {
        //        switch (control)
        //        {
        //            case 0:
        //            {
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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
        //                if (player.Inventory.HasItem(ItemNames.SoulJade))
        //                {
        //                    player.Inventory.RemoveItem(ItemNames.SoulJade);
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text(
        //                                "Thanks for helping me! I shall reward you with a MoonBox to show you my appreciation!")
        //                            .Link("Oh! Thanks a lot!.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    var moonBox = Item.CreateFor(ItemNames.Moonbox);
        //                    player.Inventory.AddItem(moonBox);
        //                    Player.Teleport(025, 025, 1042);
        //                }
        //                else
        //                {
        //                    using (var banzai = new NpcDialog(20))
        //                    {
        //                        banzai
        //                            .Text("Sorry, you do not have a SoulJade.")
        //                            .Link("That is ok.", 255)
        //                            .Finish();

        //                        Player.Send(banzai);
        //                    }
        //                    Player.Teleport(025, 025, 1042);
        //                }
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