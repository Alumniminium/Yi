using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Packets.Conquer;
using YiX.Structures;

namespace YiX.SelfContainedSystems
{
    public static class StorageSystem
    {
        public static ConcurrentDictionary<int, StorageList> StoragePool = new ConcurrentDictionary<int, StorageList>();
        public static void CreateFor(YiObj owner) => StoragePool.AddOrUpdate(owner.UniqueId, new StorageList());

        public static void NewStorage(YiObj owner, int npcId)
        {
            if (!StoragePool.ContainsKey(owner.UniqueId))
                CreateFor(owner);
            var storage = StoragePool[owner.UniqueId].FirstOrDefault(s => s.NpcId == npcId);
            if (storage == null)
            {
                storage = Storage.Create(owner, npcId);
                StoragePool[owner.UniqueId].Add(storage);
            }
            owner.CurrentStorageId = storage.StorageId;
        }

        public static IEnumerable<Storage> GetStorageListForId(YiObj owner, int npcId)
        {
            if (!StoragePool.ContainsKey(owner.UniqueId))
                yield break;
            foreach (var storage in StoragePool[owner.UniqueId].Where(storage => storage.NpcId == npcId))
                yield return storage;
        }

        public static void ShowStock(YiObj owner)
        {
            if (StoragePool[owner.UniqueId].TryGetValue(owner.CurrentStorageId, out var storage))
            {
                if (storage.Members[owner.UniqueId].HasFlag(StorageAccess.ItemSee))
                    (owner as Player)?.Send(LegacyPackets.WarehouseItems(storage.StorageId, storage.Contents));
                if (storage.Members[owner.UniqueId].HasFlag(StorageAccess.MoneySee))
                    (owner as Player)?.Send(MsgItem.Create(storage.StorageId, 0, storage.Money, MsgItemType.ShowWarehouseMoney));
            }
            else
                Output.WriteLine("Storage not found.");
        }

        public static bool PutIn(YiObj owner,int npcId, Item item)
        {
            if (StoragePool[owner.UniqueId].TryGetValue(owner.CurrentStorageId, out var storage))
            {
                if (storage.NpcId != npcId)
                {
                    Message.SendTo(owner, "hacker.", MsgTextType.Action);
                    return false;
                }
                if (storage.CanAddItems(owner))
                {
                    owner.Inventory.RemoveItem(item);
                    storage.Contents.AddOrUpdate(item.UniqueId, item);
                    return true;
                }
                Message.SendTo(owner, "You're not allowed to add items to this storage.", MsgTextType.Action);
            }
            else
                NewStorage(owner, npcId);
            return false;
        }

        public static void PutInMoney(YiObj owner, int amount)
        {
            if (!StoragePool[owner.UniqueId].TryGetValue(owner.CurrentStorageId, out var storage))
                return;

            if (storage.CanAddMoney(owner))
            {
                if (owner.Money < amount)
                    return;

                owner.Money -= amount;
                storage.Money += amount;
                (owner as Player)?.Send(MsgItem.Create(owner.UniqueId, 0, storage.Money, MsgItemType.ShowWarehouseMoney));
                return;
            }
            Message.SendTo(owner, "You're not allowed to add money to this storage.", MsgTextType.Action);
        }
        public static void TakeOutMoney(YiObj owner, int amount)
        {
            if (!StoragePool[owner.UniqueId].TryGetValue(owner.CurrentStorageId, out var storage))
                return;

            if (storage.CanTakeMoney(owner))
            {
                if (storage.Money < amount)
                    return;

                owner.Money += amount;
                storage.Money -= amount;
                (owner as Player)?.Send(MsgItem.Create(owner.UniqueId, 0, storage.Money, MsgItemType.ShowWarehouseMoney));
                return;
            }
            Message.SendTo(owner, "You're not allowed to take money from this storage.", MsgTextType.Action);
        }
        public static bool TakeOut(YiObj owner, int npcId, int itemId)
        {
            if (StoragePool[owner.UniqueId].TryGetValue(owner.CurrentStorageId, out var storage))
            {
                if (storage.NpcId != npcId)
                {
                    Message.SendTo(owner, "hacker.", MsgTextType.Action);
                    return false;
                }
                if (storage.CanTakeItems(owner))
                {
                    if (!storage.Contents.TryRemove(itemId, out var item))
                        return false;

                    if (owner.Inventory.AddItem(item))
                        return true;

                    storage.Contents.AddOrUpdate(item.UniqueId, item);
                    Message.SendTo(owner, "Item was returned. Inventory full?", MsgTextType.Action);
                }
                else
                {
                    Message.SendTo(owner, "You're not allowed to take out items from this storage.", MsgTextType.Action);
                }
            }
            else
            {
                NewStorage(owner, npcId);
            }
            return false;
        }

        public static void SetUpStorageSpaces()
        {
            //foreach (var character in Collections.Players.Values)
            //{
            //    if (!StoragePool.ContainsKey(character.UniqueId))
            //        CreateFor(character);

            //    foreach (var item in StoragePool.Values.SelectMany(poolItem => poolItem.Where(item => item.Members.ContainsKey(character.UniqueId))))
            //        StoragePool[character.UniqueId].Add(item);
            //}
            //foreach (var bot in Collections.Bots.Values)
            //{
            //    if (!StoragePool.ContainsKey(bot.UniqueId))
            //        CreateFor(bot);
            //    foreach (var poolItem in StoragePool.Values)
            //    {
            //        foreach (var item in poolItem)
            //        {
            //            if (item.Members.ContainsKey(bot.UniqueId))
            //                StoragePool[bot.UniqueId].Add(item);
            //        }
            //    }
            //}
        }
    }
}
