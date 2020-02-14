using System;
using System.Runtime.InteropServices;
using Yi.Database;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Sockets;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgItem
    {
        private ushort Size;
        private ushort Id;
        private int UnqiueId;
        private int Param;
        private MsgItemType Type;
        private int Timestamp;
        private int Value;

        public static MsgItem Create(int uid, int value, int param, MsgItemType type)
        {
            var packet = stackalloc MsgItem[1];
            {
                packet->Size = (ushort)sizeof(MsgItem);
                packet->Id = 1009;
                packet->UnqiueId = uid;
                packet->Param = param;
                packet->Type = type;
                packet->Value = value;
                packet->Timestamp = Environment.TickCount;
            }
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgItem*)p = *packet;
            return buffer;
        }

        public static void Handle(Player player, byte[] buffer)
        {
            try
            {
                fixed (byte* p = buffer)
                {
                    var packet = *(MsgItem*) p;

                    BufferPool.RecycleBuffer(buffer);

                    switch (packet.Type)
                    {
                        case MsgItemType.BuyItemAddItem:
                            BuyItem(player, ref packet);
                            break;
                        case MsgItemType.SellItem:
                            SellItem(player, ref packet);
                            break;
                        case MsgItemType.RemoveInventory:
                            DropItem(player, ref packet);
                            break;
                        case MsgItemType.EquipItem:
                            EquipItem(player, ref packet);
                            break;
                        case MsgItemType.SetEquipPosition:
                            break;
                        case MsgItemType.UnEquipItem:
                            UnEquipItem(player, ref packet);
                            break;
                        case MsgItemType.ShowWarehouseMoney:
                            StorageSystem.ShowStock(player);
                            break;
                        case MsgItemType.DepositWarehouseMoney:
                            StorageSystem.PutInMoney(player, packet.Param);
                            break;
                        case MsgItemType.WithdrawWarehouseMoney:
                            StorageSystem.TakeOutMoney(player, packet.Param);
                            break;
                        case MsgItemType.DropGold:
                            DropGold(player, ref packet);
                            break;
                        case MsgItemType.RepairItem:
                            RepairItem(player, ref packet);
                            break;
                        case MsgItemType.UpgradeDragonball:
                            UpgradeDragonball(player, ref packet);
                            break;
                        case MsgItemType.UpgradeMeteor:
                            UpgradeMeteor(player, ref packet);
                            break;
                        case MsgItemType.ShowVendingList:
                            BoothSystem.Show(player, packet.UnqiueId);
                            break;
                        case MsgItemType.AddVendingItem:
                            AddItemToShop(player, ref packet);
                            break;
                        case MsgItemType.RemoveVendingItem:
                            RemoveItemFromShop(player, ref packet);
                            break;
                        case MsgItemType.BuyVendingItem:
                            BuyBoothItem(player, ref packet);
                            break;
                        case MsgItemType.Ping:
                            Ping(player, ref packet);
                            break;
                        case MsgItemType.Enchant:
                            UpgradeItemEnchant(player, ref packet);
                            break;
                        case MsgItemType.BoothAddCp:
                            player.GetMessage("Server", player.Name.TrimEnd('\0'), "This server doesn't use conquer points as currency.", MsgTextType.Action);
                            break;
                        default:
                            Output.WriteLine(((byte[]) packet).HexDump());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        private static void RepairItem(Player player, ref MsgItem packet)
        {
            var found = player.Inventory.FindByUID(packet.UnqiueId);
            if (found == null)
                return;

            if (found.CurrentDurability >= found.MaximumDurability)
                return;

            if (Collections.Items.TryGetValue(found.ItemId, out var original))
            {
                var delta = Math.Max(0, found.MaximumDurability - found.CurrentDurability);
                var cost = (double)original.PriceBaseline * delta / found.MaximumDurability;

                switch (found.ItemId % 10)
                {
                    case 9:
                        cost *= 1.125;
                        break;
                    case 8:
                        cost *= 0.975;
                        break;
                    case 7:
                        cost *= 0.9;
                        break;
                    case 6:
                        cost *= 0.825;
                        break;
                    default:
                        cost *= 0.75;
                        break;
                }

                cost = Math.Max(0, Math.Round(cost, 0));

                if (player.Money >= cost)
                {
                    player.Money -= (int)cost;
                    Message.SendTo(player, $"You paid: {cost}.", MsgTextType.Action);
                }
                else
                {
                    Message.SendTo(player, $"You don't have enough money (Cost: {cost}).", MsgTextType.Action);
                    return;
                }
            }

            found.CurrentDurability = found.MaximumDurability;
            var pack = new MsgItemInformation(found, MsgItemInfoAction.Update);
            player.ForceSend(pack, pack.Size);
        }


        private static void BuyBoothItem(Player player, ref MsgItem packet)
        {
            if (BoothSystem.Buy(player, packet.Param, packet.UnqiueId))
                player.Send(packet);
        }

        private static void RemoveItemFromShop(Player player, ref MsgItem packet)
        {
            BoothSystem.Remove(player, packet.UnqiueId);
            ScreenSystem.Send(player, packet, true);
        }

        private static void AddItemToShop(Player player, ref MsgItem packet)
        {
            BoothSystem.Add(player, packet.UnqiueId, packet.Param);
            ScreenSystem.Send(player, packet, true);
        }

        private static void Ping(Player player, ref MsgItem packet)
        {
            player.ForceSend(packet, packet.Size);
            player.ForceSend(MsgTick.Create(player), (ushort)sizeof(MsgTick));
        }

        private static void DropGold(Player player, ref MsgItem packet)
        {
            if (player.Money >= packet.UnqiueId)
            {
                FloorItemSystem.DropMoney(null, player, packet.UnqiueId);
            }
            player.Send(packet);
        }

        private static void DropItem(Player player, ref MsgItem packet)
        {
            if (player.Inventory.Items.TryRemove(packet.UnqiueId, out var found))
            {
                FloorItemSystem.Drop(null, player, found);
                player.Send(packet);
            }
        }

        private static void UnEquipItem(Player player, ref MsgItem packet)
        {
            if (player.Equipment.Unequip((MsgItemPosition)packet.Param))
                player.Send(packet);
        }

        private static void EquipItem(Player player, ref MsgItem packet)
        {
            var found = player.Inventory.FindByUID(packet.UnqiueId);
            if (found != null)
            {
                if (ScriptEngine.Scripts.TryGetValue(ScriptType.ItemUsage, out var sc) && sc.Execute(player, found))
                    return;

                Message.SendTo(player, $"Item {found.ItemId} has no script - Equipping it instead at {(MsgItemPosition)packet.Param}", MsgTextType.Action);

                if (player.Equipment.Equip(found, (MsgItemPosition) packet.Param))
                {
                    packet.Type = MsgItemType.SetEquipPosition;
                    player.Send(packet);
                }
            }
        }

        private static void SellItem(Player player, ref MsgItem packet)
        {
            var found = player.Inventory.FindByUID(packet.Param);
            if (found != null)
            {
                if (player.Inventory.Items.TryRemove(found.UniqueId, out found))
                    player.Money += found.PriceBaseline;
            }
            packet.UnqiueId = packet.Param;
            packet.Param = 0;
            packet.Type = MsgItemType.RemoveInventory;
            player.Send(packet);
        }

        private static void BuyItem(Player player, ref MsgItem packet)
        {
            if (Collections.Items.TryGetValue(packet.Param, out var item))
            {
                if (GameWorld.Find(packet.UnqiueId, out Npc shop))
                {
                    if (shop.Inventory == null)
                    {
                        Output.WriteLine($"Shop {shop.UniqueId} null.");
                        return;
                    }
                    if (!shop.Inventory.HasItem(item.ItemId))
                    {
                        Output.WriteLine($"Item {item.ItemId} not found in Shop {shop.UniqueId}");
                        return;
                    }

                    if (player.Money >= item.PriceBaseline)
                    {
                        if (player.Inventory.Count < 40)
                        {
                            var cloned = CloneChamber.Clone(item);
                            cloned.UniqueId = YiCore.Random.Next(1000, 100000);
                            player.Inventory.Items.AddOrUpdate(cloned.UniqueId, cloned);
                            player.Money -= cloned.PriceBaseline;
                            player.Send(new MsgItemInformation(cloned, MsgItemPosition.Inventory));
                        }
                    }
                }
            }
        }
        private static void UpgradeMeteor(Player player, ref MsgItem packet)
        {
            var mainItem = player.Inventory.FindByUID(packet.UnqiueId);//item to upgrade
            var subItem = player.Inventory.FindByUID(packet.Param);//met
            if (mainItem != null && subItem != null)
            {
                var jmp = 0;
                var itemId = mainItem.ItemId;
                var itemType = itemId / 10000;
                CheckValidID:
                switch (itemType)
                {
                    case 11: // head
                    case 12: // neck
                    case 15: // ring
                    case 13: // armor
                    case 16: // boots
                        itemId += 10;
                        break;
                    default:
                            if (itemType >= 40 && itemType <= 61) // weapons
                                itemId += 10;
                            break;
                }
                if (itemId != mainItem.ItemId)
                {
                    if (Collections.Items.ContainsKey(itemId))
                    {
                        var itemQuality = (byte)(mainItem.ItemId % 10);
                        if (itemQuality >= 3 && itemQuality <= 9)
                        {
                            var lucky = false;
                            var num = YiCore.Random.Next(1, 1000);
                            if (itemQuality < 6) lucky = num <= 950;
                            else if (itemQuality == 6)lucky = num <= 880;
                            else if (itemQuality == 7)lucky = num <= 750;
                            else if (itemQuality >= 8) lucky = num <= 670;
                           
                            if (lucky)
                            {
                                Message.SendTo(player, "You have successfully upgraded your item\'s level!", MsgTextType.Action);
                                if (num < 10 && mainItem.Gem1 == 0)
                                    mainItem.Gem1 = 255;
                                else if (num == 1 && mainItem.Gem2 == 0)
                                    mainItem.Gem2 = 255;

                                mainItem.ItemId = itemId;
                                player.Inventory.RemoveItem(mainItem);//remove the main item
                                player.Inventory.RemoveItem(subItem);//remove the met
                                player.Inventory.AddOrUpdate(mainItem.UniqueId, mainItem);//update the new item
                                player.Send(new MsgItemInformation(mainItem, MsgItemPosition.Inventory));
                            }
                            else Message.SendTo(player, "Your item failed to upgrade.", MsgTextType.Action);
                        }
                        else
                            Output.WriteLine("The item quality has recieved an else in MsgItem.cs with a value of " + itemQuality);
                    }
                    else if (jmp < 3)
                    {
                        jmp++;
                        goto CheckValidID;
                    }
                }
                else Message.SendTo(player, "You can\'t upgrade the level of this item.", MsgTextType.Action);
            }
            else Message.SendTo(player, "Invalid items were attempted.", MsgTextType.Action);
        }

        private static void UpgradeDragonball(Player player, ref MsgItem packet)
        {
            var mainItem = player.Inventory.FindByUID(packet.UnqiueId);//item to upgrade
            var subItem = player.Inventory.FindByUID(packet.Param);//db
            if (mainItem != null && subItem != null)
            {
                if (subItem.ItemId == 1088000)
                {
                    var itemQuality = (byte)(mainItem.ItemId % 10);
                    if (itemQuality >= 3 && itemQuality < 9)
                    {
                        player.Inventory.RemoveItem(mainItem);//remove the item to update
                        player.Inventory.RemoveItem(subItem);//remove the db before attempting
                        var lucky = true;
                        var num = YiCore.Random.Next(5);
                        if (itemQuality < 6)lucky = num <= 90;
                        else if (itemQuality == 6)lucky = num <= 75;
                        else if (itemQuality == 7)lucky = num <= 50;
                        else if (itemQuality == 8) lucky = num <= 35;

                        if (num == 1 && mainItem.Gem1 == 0)
                            mainItem.Gem1 = 255;
                        else if (num == 1 && YiCore.Random.Next(3) == 3)
                            mainItem.Gem2 = 255;

                        if (lucky)
                        {
                            Message.SendTo(player, "You have successfully upgraded your item\'s quality!", MsgTextType.Action);
                            if (itemQuality < 3) mainItem.ItemId += 2;//so it goes straight to refined
                            mainItem.ItemId++;
                        }
                        else Message.SendTo(player, "You have failed to upgrade your item\'s quality.", MsgTextType.Action);

                        player.Inventory.AddOrUpdate(mainItem.UniqueId, mainItem);//RETURN THE ITEM TO THE PLAYER REGARDLESS OF UPGRADE
                        player.Send(new MsgItemInformation(mainItem, MsgItemPosition.Inventory));
                    }
                    else Message.SendTo(player, "Invalid items were attempted.", MsgTextType.Action);
                }
            }
        }

        private static int GetGemBlessWorth(int itemId)//FOR ENCHANTMENT
        {
            var high = 1;
            var low = 0;
            if (itemId % 10 == 1) // refined
            {
                high = 59;
                low = 1;
            }
            else
            {
                switch (itemId)
                {
                    /* unique ---------------------------------------------------unique-----------------------------*/
                    case 700012:                         high = 159; low = 100; break;// dragon
                    case 700002:case 700062:case 700052: high = 109; low = 60; break;// phoenix, moon, violet
                    case 700032:                         high = 129; low = 80; break; // rainbow
                    case 700072:case 700042:case 700022: high = 89; low = 40; break;// tortoise, kylin, fury
                    /* super -------------------------------------------------------super---------------------------*/
                    case 700013:                         high = 255; low = 200; break; // dragon
                    case 700003: case 700073:case 700033:high = 229; low = 170; break;// phoenix, tortoise, rainbow
                    case 700063: case 700053:            high = 199; low = 140; break;// moon, violet
                    case 700023:                         high = 149; low = 90; break;// fury
                    case 700043:                         high = 119; low = 70; break;// kylin
                }
            }
            return YiCore.Random.Next(low, high);
        }

        private static void UpgradeItemEnchant(Player player, ref MsgItem packet)
        {
            var mainItem = player.Inventory.FindByUID(packet.UnqiueId);

            var subItem = player.Inventory.FindByUID(packet.Param);
            if (mainItem != null && subItem != null)
            {
                if (subItem.ItemId / 1000 == 700)//gem id const
                {
                    var num = GetGemBlessWorth(subItem.ItemId);
                    if (num > mainItem.Enchant)
                        mainItem.Enchant = (byte)num;

                    player.Inventory.RemoveItem(mainItem);//remove the item to update
                    player.Inventory.RemoveItem(subItem);//remove the db before attempting
                    player.Inventory.AddOrUpdate(mainItem.UniqueId, mainItem);//RETURN THE ITEM TO THE PLAYER REGARDLESS OF UPGRADE
                    player.Send(new MsgItemInformation(mainItem, MsgItemPosition.Inventory));
                }
            }
        }

        public static implicit operator byte[](MsgItem msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgItem*)p = *&msg;
            return buffer;
        }

        public static implicit operator MsgItem(byte[] msg)
        {
            fixed (byte* p = msg)
                return *(MsgItem*)p;
        }
    }
}