using System.Collections.Concurrent;
using System.Linq;
using YiX.Database;
using YiX.Entities;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Packets.Conquer;
using YiX.World;

namespace YiX.SelfContainedSystems
{
    public static class BoothSystem
    {
        public static ConcurrentDictionary<int, ConcurrentDictionary<int, Product>> BoothPool = new ConcurrentDictionary<int, ConcurrentDictionary<int, Product>>();

        public static void CreateFor(YiObj owner)
        {
            if (owner is Npc npc)
            {
                npc.Type = 2;
                owner.BoothId = 10000000 + owner.UniqueId;
                BoothPool.AddOrUpdate(owner.BoothId, new ConcurrentDictionary<int, Product>());
                foreach (var item in owner.Inventory)
                {
                    var product = new Product(owner.UniqueId, item.Value.PriceBaseline, owner.BoothId, item.Value);
                    Add(owner, item.Key, product.Price);
                }
            }
            else
            {
                owner.BoothId = owner.BoothId;
                BoothPool.AddOrUpdate(owner.BoothId, new ConcurrentDictionary<int, Product>());

                (owner as Player)?.Send(MsgStatus.Create(owner.MapId, 0x0010));
                (owner as Player)?.Send(LegacyPackets.SpawnCarpet((Player)owner, owner.BoothId));
            }
        }

        public static void DestroyFor(YiObj owner) => BoothPool.TryRemove(owner.UniqueId);

        public static bool Add(YiObj owner, int uniqueId, int price)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                CreateFor(owner);

            if (!owner.Inventory.TryGetItem(uniqueId, out var item))
                return false;
            var product = new Product(owner.UniqueId, price, owner.BoothId, item);
            (owner as Player)?.Send(MsgItemInfoEx.CreateBoothItem(product));
            return BoothPool[owner.BoothId].TryAdd(item.UniqueId, product);
        }
        public static bool Add(YiObj owner, Item item, int price)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                CreateFor(owner);

            var product = new Product(owner.UniqueId, price, owner.BoothId, item);
            (owner as Player)?.Send(MsgItemInfoEx.CreateBoothItem(product));
            return BoothPool[owner.BoothId].TryAdd(item.UniqueId, product);
        }
        public static bool Remove(YiObj owner, int uniqueId)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                CreateFor(owner);

            if (!owner.Inventory.TryGetItem(uniqueId, out var product))
                return false;

            return BoothPool[owner.BoothId].TryRemove(product.UniqueId);
        }

        public static void Show(Player player, int uniqueId)
        {
            if (!GameWorld.Find(uniqueId - 10000000, out YiObj owner))
                return;

            var ordered = BoothPool[owner.BoothId].OrderByDescending(kvp => kvp.Value.Item.ItemId);
            foreach (var product in ordered)
                player.Send(MsgItemInfoEx.CreateBoothItem(product.Value));
        }

        public static bool Buy(Player player, int shopId, int itemId)
        {
            var last = false;
            var ownerId = BoothPool.Keys.FirstOrDefault(c => c == shopId);

            if (!GameWorld.Find(ownerId - 10000000, out YiObj owner))
                return false;

            if (!owner.Inventory.TryGetItem(itemId, out var item) || !BoothPool[ownerId].ContainsKey(item.UniqueId))
                return false;

            var price = BoothPool[ownerId][item.UniqueId].Price;

            if (player.Money < price)
                return false;

            if (player.Inventory.Count >= 40)
                return false;

            player.Money -= price;
            owner.Money += price;
            if (item.StackAmount == 1)
            {
                Remove(owner, itemId);
                owner.Inventory.RemoveItem(item);
                last = true;
            }
            else
            {
                item.StackAmount--;
                //item.Price += item.Price / 2;
            }
            var purchased = CloneChamber.Clone(item);
            purchased.StackAmount = 1;
            player.Inventory.AddItem(purchased);
            Show(player, shopId);
            return last;
        }

        public static void SetUpBooths()
        {
            foreach (var yiObj in Collections.Npcs.Where(yiObj => yiObj.Value.Inventory?.Count > 0))
                CreateFor(yiObj.Value);
        }
    }
}
