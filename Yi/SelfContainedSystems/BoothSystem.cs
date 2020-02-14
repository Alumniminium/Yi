using System.Collections.Concurrent;
using System.Linq;
using Yi.Database;
using Yi.Entities;
using Yi.Helpers;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.World;

namespace Yi.SelfContainedSystems
{
    public static class BoothSystem
    {
        public static ConcurrentDictionary<int, ConcurrentDictionary<int,Item>> BoothPool = new ConcurrentDictionary<int, ConcurrentDictionary<int,Item>>();

        public static void Create(YiObj owner)
        {
            if (owner is Npc npc)
            {
                npc.Type = 2;
                owner.BoothId = 10000000 + owner.UniqueId;
                BoothPool.AddOrUpdate(owner.BoothId, new ConcurrentDictionary<int, Item>());
                foreach (var item in owner.Inventory)
                {
                    item.Value.StackAmount = 255;
                    item.Value.Product = new Product(owner.UniqueId, item.Value.PriceBaseline, owner.BoothId);
                    Add(owner, item.Key, item.Value.Product.Price);
                }
            }
            else
            {
                owner.BoothId = owner.BoothId;
                BoothPool.AddOrUpdate(owner.BoothId, new ConcurrentDictionary<int, Item>());

                (owner as Player)?.Send(MsgStatus.Create(owner.MapId, 0x0010));
                (owner as Player)?.Send(LegacyPackets.SpawnCarpet((Player) owner, owner.BoothId));
            }
        }

        public static void Destroy(YiObj owner) => BoothPool.TryRemove(owner.UniqueId);

        public static bool Add(YiObj owner, int uniqueId, int price)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                Create(owner);

            var item = owner.Inventory.FindByUID(uniqueId);
            item.Product = new Product(owner.UniqueId, price, owner.BoothId);
            (owner as Player)?.Send(MsgItemInfoEx.CreateBoothItem(item));
            return BoothPool[owner.BoothId].TryAdd(item.UniqueId, item);
        }
        public static bool Add(YiObj owner, Item item, int price)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                Create(owner);

            item.Product = new Product(owner.UniqueId, price, owner.BoothId);
            (owner as Player)?.Send(MsgItemInfoEx.CreateBoothItem(item));
            return BoothPool[owner.BoothId].TryAdd(item.UniqueId, item);
        }
        public static bool Remove(YiObj owner, int uniqueId)
        {
            if (!BoothPool.ContainsKey(owner.BoothId))
                Create(owner);

            var product = owner.Inventory.FindByUID(uniqueId);
            return product != null && BoothPool[owner.BoothId].TryRemove(product.UniqueId);
        }

        public static void Show(Player player, int uniqueId)
        {
            if (!GameWorld.Find(uniqueId- 10000000, out YiObj owner))
                return;

            var ordered = BoothPool[owner.BoothId].OrderByDescending(kvp => kvp.Value.ItemId);
            foreach (var product in ordered)
                player.Send(MsgItemInfoEx.CreateBoothItem(product.Value));
        }

        public static bool Buy(Player player, int shopId, int productId)
        {
            var last = false;
            var ownerId = BoothPool.Keys.FirstOrDefault(c => c == shopId);

            if (!GameWorld.Find(ownerId- 10000000, out YiObj owner))
                return false;

            var product = owner?.Inventory.FindByUID(productId);

            if (product == null || !BoothPool[ownerId].ContainsKey(product.UniqueId))
                return false;

            var price = BoothPool[ownerId][product.UniqueId].Product.Price;

            if (player.Money < price)
                return false;

            if (player.Inventory.Count >= 40)
                return false;
            
            player.Money -= price;
            owner.Money += price;
            if (product.StackAmount == 1)
            {
                Remove(owner, productId);
                owner.Inventory.RemoveItem(product);
                last = true;
            }
            else
            {
                product.StackAmount--;
                product.Product.Price += product.Product.Price/2;
            }
            var purchased = CloneChamber.Clone(product);
            purchased.StackAmount = 1;
            player.Inventory.AddItem(purchased);
            Show(player, shopId);
            return last;
        }

        public static void SetUpBooths()
        {
            foreach (var yiObj in Collections.Npcs.Where(yiObj => yiObj.Value.Inventory?.Count > 0))
                Create(yiObj.Value);
        }
    }
}
