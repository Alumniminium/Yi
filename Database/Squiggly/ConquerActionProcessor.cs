using System;
using System.Linq;
using YiX.Database.Squiggly.Models;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Packets.Conquer;
using YiX.SelfContainedSystems;
using YiX.Structures;

namespace YiX.Database.Squiggly
{
    public static class ConquerActionProcessor
    {
        public static void ExecuteAction(Monster target, YiObj attacker)
        {
            using (var db = new SquigglyContext())
            {
                var cqaction = db.cq_action.Find((long)target.CQAction);
                Process(target, attacker, cqaction, db);
            }
        }
        public static void ExecuteAction(Npc target, Player attacker, long task = 0)
        {
            using (var db = new SquigglyContext())
            {
                if (task == 0)
                {
                    var cqtask = db.cq_task.Find(target.Task0);
                    var cqaction = db.cq_action.Find(cqtask.id_next);
                    Process(target, attacker, cqaction, db);
                }
                else
                {
                    var cqtask = db.cq_task.Find(task);
                    var cqaction = db.cq_action.Find(cqtask.id_next);
                    Process(target, attacker, cqaction, db);
                }
            }
        }

        private static void Process(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            if (cqaction == null || cqaction.id == 0)
                return;

            invoker.CurrentAction = cqaction;
            var type = (Cq_ActionId)cqaction.type;
            //Output.WriteLine($"Mob Action -> Type: {type}:{(int) type} Data: {cqaction.data} Param: {cqaction.param.Trim()}");

            switch (type)
            {
                default:
                    {
                        Output.WriteLine($"Unknown Cq_ActionId -> {cqaction}", ConsoleColor.Blue);
                        break;
                    }
                case Cq_ActionId.ACTION_MENUTEXT:
                    {
                        if (invoker is Player player)
                        {
                            player.Send(LegacyPackets.NpcSay(cqaction.param.Trim().Replace("~", " ")));
                            if (cqaction.id_next == 0)
                                player.Send(LegacyPackets.NpcFinish());
                            else
                                Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
                        }

                        break;
                    }
                case Cq_ActionId.ACTION_MENULINK:
                    {
                        if (invoker is Player player)
                        {
                            var option = cqaction.param.Trim().Split(' ')[0];
                            var sControl = cqaction.param.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            var control = int.Parse(sControl[1]);

                            player.NpcTasks.Add((byte)player.NpcTasks.Count, (int)control);

                            player.Send(LegacyPackets.NpcLink(option.Replace("~", " "), (byte)(player.NpcTasks.Count - 1)));
                            if (cqaction.id_next == 0)
                                player.Send(LegacyPackets.NpcFinish());
                            else
                                Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
                        }

                        break;
                    }
                case Cq_ActionId.ACTION_MENUPIC:
                    {
                        if (invoker is Player player)
                        {
                            var faceId = byte.Parse(cqaction.param.Trim().Split(' ')[2]);
                            player.Send(LegacyPackets.NpcFace(faceId));
                            if (cqaction.id_next == 0)
                                player.Send(LegacyPackets.NpcFinish());
                            else
                                Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
                        }
                        break;
                    }
                case Cq_ActionId.ACTION_MENUCREATE:
                    {
                        if (invoker is Player player)
                        {
                            player.Send(LegacyPackets.NpcFinish());
                        }
                        break;
                    }
                case Cq_ActionId.ACTION_USER_SEX:
                    {
                        if (cqaction.id_nextfail == 0 && cqaction.id_next == 0)
                            return;

                        //If male next_id else nextid_fail

                        break;
                    }
                case Cq_ActionId.ACTION_ITEM_CHECK:
                    {
                        ACTION_ITEM_CHECK(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_MST_DROPITEM:
                    {
                        ACTION_MST_DROPITEM(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_RAND:
                    {
                        ACTION_RAND(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_ATTR:
                    {
                        ACTION_USER_ATTR(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_RANDACTION:
                    {
                        ACTION_RANDACTION(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_TALK:
                    {
                        invoker.GetMessage("SYSTEM", invoker.Name.Trim(), cqaction.param.Trim().Replace("~", ""), (MsgTextType)cqaction.data);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_HAIR:
                    {
                        ACTION_USER_HAIR(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_MEDIAPLAY:
                    {
                        ACTION_USER_MEDIAPLAY(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_EFFECT:
                    {
                        ACTION_USER_EFFECT(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_USER_CHGMAP:
                    {
                        ACTION_USER_CHGMAP(target, invoker, cqaction, db);
                        break;
                    }
                case Cq_ActionId.ACTION_ITEM_LEAVESPACE:
                    ACTION_ITEM_LEAVESPACE(target, invoker, cqaction, db);
                    break;
                case Cq_ActionId.ACTION_ITEM_ADD:
                    ACTION_ITEM_ADD(target, invoker, cqaction, db);
                    break;
                case Cq_ActionId.ACTION_ITEM_MULTICHK:
                    ACTION_ITEM_MULTICHK(target, invoker, cqaction, db);
                    break;
                case Cq_ActionId.ACTION_ITEM_MULTIDEL:
                    ACTION_ITEM_MULTIDEL(target, invoker, cqaction, db);
                    break;
                case Cq_ActionId.ACTION_CHKTIME:
                    ACTION_CHKTIME(target, invoker, cqaction, db);
                    break;
                case Cq_ActionId.ACTION_USER_MAGIC:
                    ACTION_USER_MAGIC(target, invoker, cqaction, db);
                    break;
            }
        }

        private static void ACTION_USER_MAGIC(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var param = cqaction.param.Trim().Split(' ');
            var skillId = ushort.Parse(param[1]);
            switch (param[0])
            {
                case "learn":
                    {
                        (invoker as Player)?.AddSkill(new Skill(skillId, 0, 0));
                        break;
                    }
            }
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_CHKTIME(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            if (cqaction.data == 3)
            {
                var param = cqaction.param.Trim().Split(' ');
                var startDayOfWeek = int.Parse(param[0]);
                var startTime = param[1];
                var endDayOfWeek = int.Parse(param[2]);
                var endTime = param[3];

                if (((int)DateTime.Now.DayOfWeek) >= startDayOfWeek && ((int)DateTime.Now.DayOfWeek) <= endDayOfWeek)
                {
                    var startDT = DateTime.ParseExact(startTime, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    var endDT = DateTime.ParseExact(endTime, "H:mm", null, System.Globalization.DateTimeStyles.None);

                    if (DateTime.Now.Hour >= startDT.Hour && DateTime.Now.Hour <= endDT.Hour)
                    {
                        Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
                        return;
                    }

                }
            }
            Process(target, invoker, db.cq_action.Find(cqaction.id_nextfail), db);
        }

        private static void ACTION_ITEM_MULTIDEL(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var items = cqaction.param.Trim().Split(' ');

            var startId = int.Parse(items[0]);
            var endId = int.Parse(items[1]);
            var amount = int.Parse(items[2]);

            var range = Enumerable.Range(startId, (endId - startId) + 1);


            var amountFound = (from id in range from inventoryItem in invoker.Inventory.Items where inventoryItem.Value.ItemId == id select id).Count();
            var removed = 0;
            foreach (var inventoryItem in invoker.Inventory.Items)
            {
                if (removed == amount)
                    break;
                if (range.Contains(inventoryItem.Value.ItemId))
                {
                    invoker.Inventory.RemoveItem(inventoryItem.Value);
                    removed++;
                }
            }


            Process(target, invoker, amountFound >= amount
                ? db.cq_action.Find(cqaction.id_next)
                : db.cq_action.Find(cqaction.id_nextfail), db);
        }
        private static void ACTION_ITEM_MULTICHK(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var items = cqaction.param.Trim().Split(' ');

            var startId = int.Parse(items[0]);
            var endId = int.Parse(items[1]);
            var amount = int.Parse(items[2]);

            var range = Enumerable.Range(startId, (endId - startId) + 1);


            var amountFound = (from id in range from inventoryItem in invoker.Inventory.Items where inventoryItem.Value.ItemId == id select id).Count();

            Process(target, invoker, amountFound >= amount
                ? db.cq_action.Find(cqaction.id_next)
                : db.cq_action.Find(cqaction.id_nextfail), db);
        }

        private static void ACTION_ITEM_ADD(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            invoker.Inventory.AddItem(ItemFactory.Create(721189));
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_ITEM_LEAVESPACE(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            if (invoker.Inventory.Count + cqaction.data < 40)
            {
                Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
            }
            else
            {
                Process(target, invoker, db.cq_action.Find(cqaction.id_nextfail), db);
            }
        }

        private static void ACTION_USER_CHGMAP(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var nextIds = cqaction.param.Trim().Split(' ');
            var map = ushort.Parse(nextIds[0]);
            var x = ushort.Parse(nextIds[1]);
            var y = ushort.Parse(nextIds[2]);
            invoker.Teleport(x, y, map);
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_USER_EFFECT(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var nextIds = cqaction.param.Trim().Split(' ');
            (invoker as Player)?.Send(LegacyPackets.Effect(invoker, nextIds[1]));
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_USER_MEDIAPLAY(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var nextIds = cqaction.param.Trim().Split(' ');
            //Send Sound Packet here
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_USER_HAIR(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var nextIds = cqaction.param.Trim().Split(' ');
            invoker.Hair = uint.Parse(nextIds[1]);
            Process(target, invoker, db.cq_action.Find(cqaction.id_next), db);
        }

        private static void ACTION_ITEM_CHECK(YiObj target, YiObj invoker, cq_action cqaction, SquigglyContext db)
        {
            var itemId = cqaction.data;

            if (invoker.Inventory.HasItem(itemId))
            {
                Process(target, invoker, db.cq_action.Find(cqaction.id_next),
                    db);
            }
            else
            {
                Process(target, invoker, db.cq_action.Find(cqaction.id_nextfail), db);
            }
        }

        private static void ACTION_RANDACTION(YiObj target, YiObj attacker, cq_action cqaction, SquigglyContext db)
        {
            var nextIds = cqaction.param.Trim().Split(' ');

            var nextIndex = SafeRandom.Next(nextIds.Length);

            var nextId = long.Parse(nextIds[nextIndex]);

            cqaction = db.cq_action.Find(nextId);
            //Output.WriteLine($"Mob Action -> Data: {cqaction.data} Param: {cqaction.param.Trim()}",ConsoleColor.Green);

            var dropId = cqaction.param.Trim().Split(' ')[1];
            var item = ItemFactory.Create(int.Parse(dropId));
            FloorItemSystem.Drop(attacker, target, item);
        }

        private static void ACTION_USER_ATTR(YiObj target, YiObj attacker, cq_action cqaction, SquigglyContext db)
        {
            var condition = cqaction.param.Trim();
            var what = condition.Split(' ')[0];
            switch (what)
            {
                case "level":
                    {
                        var op = condition.Split(' ')[1];
                        var val = condition.Split(' ')[2];

                        switch (op)
                        {
                            case "<=":
                                {
                                    if (attacker.Level <= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">=":
                                {
                                    if (attacker.Level >= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "<":
                                {
                                    if (attacker.Level < byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">":
                                {
                                    if (attacker.Level > byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "==":
                                {
                                    if (attacker.Level == byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unknown Operator for " + what + " -> " + op);
                                break;
                        }

                        break;
                    }
                case "life":
                    {
                        var op = condition.Split(' ')[1];
                        var val = condition.Split(' ')[2];

                        switch (op)
                        {
                            case "<=":
                                {
                                    if (attacker.CurrentHp <= int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next),
                                            db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">=":
                                {
                                    if (attacker.CurrentHp >= int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "<":
                                {
                                    if (attacker.CurrentHp < int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">":
                                {
                                    if (attacker.CurrentHp > int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "==":
                                {
                                    if (attacker.CurrentHp == int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unknown Operator for " + what + " -> " + op);
                                break;
                        }

                        break;
                    }
                case "metempsychosis":
                    {
                        var op = condition.Split(' ')[1];
                        var val = condition.Split(' ')[2];

                        switch (op)
                        {
                            case "<=":
                                {
                                    if (attacker.Reborn <= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">=":
                                {
                                    if (attacker.Reborn >= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "<":
                                {
                                    if (attacker.Reborn < byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">":
                                {
                                    if (attacker.Reborn > byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "==":
                                {
                                    if (attacker.Reborn == byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next),
                                           db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int) type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker,
                                            db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unknown Operator for " + what + " -> " + op);
                                break;
                        }

                        break;
                    }
                case "profession":
                    {
                        var op = condition.Split(' ')[1];
                        var val = condition.Split(' ')[2];

                        switch (op)
                        {
                            case "<=":
                                {
                                    if (attacker.Class <= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "<":
                                {
                                    if (attacker.Class < byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">=":
                                {
                                    if (attacker.Class >= byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">":
                                {
                                    if (attacker.Class > byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "==":
                                {
                                    if (attacker.Class == byte.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unknown Operator for " + what + " -> " + op);
                                break;
                        }

                        break;
                    }
                case "money":
                    {
                        var op = condition.Split(' ')[1];
                        var val = condition.Split(' ')[2];

                        switch (op)
                        {
                            case "+=":
                                {
                                    attacker.Money += int.Parse(val);
                                    Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    break;
                                }
                            case "<=":
                                {
                                    if (attacker.Money <= int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "<":
                                {
                                    if (attacker.Money < int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">=":
                                {
                                    if (attacker.Money >= int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case ">":
                                {
                                    if (attacker.Money > int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            case "==":
                                {
                                    if (attacker.Money == int.Parse(val))
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Green);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                                    }
                                    else
                                    {
                                        //Output.WriteLine($"{type}:{(int)type} -> {condition}", ConsoleColor.Red);
                                        Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                                    }

                                    break;
                                }
                            default:
                                Console.WriteLine("Unknown Operator for " + what + " -> " + op);
                                break;
                        }

                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown ACTION_USER_ATTR -> " + what);
                        break;
                    }
            }
        }

        private static void ACTION_RAND(YiObj target, YiObj attacker, cq_action cqaction, SquigglyContext db)
        {
            var amount = float.Parse(cqaction.param.Trim().Split(' ')[0]);
            var afterKills = float.Parse(cqaction.param.Trim().Split(' ')[1]);
            if (YiCore.Success(afterKills / amount))
            {
                //Output.WriteLine($"{type}:{(int) type} -> Chance: {afterKills/amount}", ConsoleColor.Green);
                Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
            }
            else
            {
                //Output.WriteLine($"{type}:{(int) type} -> Chance: {afterKills / amount}", ConsoleColor.Red);
                Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
            }
        }

        private static void ACTION_MST_DROPITEM(YiObj target, YiObj attacker, cq_action cqaction, SquigglyContext db)
        {
            var condition = cqaction.param.Trim();
            var what = condition.Split(' ')[0];
            switch (what)
            {
                case "dropmoney":
                    {
                        var maxAmount = int.Parse(condition.Split(' ')[1]);
                        var chance = int.Parse(condition.Split(' ')[2]) / 100;


                        if (YiCore.Success(chance))
                        {
                            //Output.WriteLine($"{type}:{(int) type} -> {maxAmount} {chance}", ConsoleColor.Green);
                            Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                            FloorItemSystem.DropMoney(attacker, target, maxAmount);
                        }
                        else
                        {
                            //Output.WriteLine($"{type}:{(int) type} -> {maxAmount} {chance}", ConsoleColor.Red);
                            Process(target, attacker, db.cq_action.Find(cqaction.id_nextfail), db);
                        }

                        break;
                    }
                case "dropitem":
                    {
                        var id = int.Parse(condition.Split(' ')[1]);
                        //Output.WriteLine($"{type}:{(int) type} -> {id}", ConsoleColor.Green);
                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                        FloorItemSystem.Drop(attacker, target, ItemFactory.Create(id));
                        Process(target, attacker, db.cq_action.Find(cqaction.id_next), db);
                        break;
                    }
            }
        }
    }
}