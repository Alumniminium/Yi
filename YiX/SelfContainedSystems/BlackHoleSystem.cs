using System.Collections.Concurrent;
using System.Collections.Generic;
using YiX.Entities;
using YiX.Helpers;
using YiX.Items;

namespace YiX.SelfContainedSystems
{
    public static class BlackHoleSystem
    {
        public static ConcurrentDictionary<int, Player> Entities = new ConcurrentDictionary<int, Player>();

        public static void Create(Player player) => Entities.AddOrUpdate(player.UniqueId, player);

        public static void Suck()
        {
            foreach (var entity in Entities)
            {
                var amount = 0;
                var GoldDrops = new List<FloorItem>();
                foreach (var item in ScreenSystem.GetEntities(entity.Value))
                {
                    if (item is FloorItem)
                    {
                        var floorItem = item as FloorItem;

                        if (floorItem.Amount > 0)
                        {
                            GoldDrops.Add(floorItem);
                        }
                    }
                }
                foreach (var floorItem in GoldDrops)
                {
                    amount += floorItem.Amount;
                    floorItem.Jobs.Clear();
                    floorItem.Destroy();
                }
                FloorItemSystem.DropMoney(entity.Value, entity.Value, amount);
            }
        }
    }
}
