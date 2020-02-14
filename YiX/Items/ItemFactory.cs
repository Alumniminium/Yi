using System;
using Newtonsoft.Json;
using YiX.Database;
using YiX.Enums;
using YiX.Helpers;
using YiX.SelfContainedSystems;

namespace YiX.Items
{
    public static class ItemFactory
    {
        public static Item CreateMoney(int amount) => Create(IdFromAmount(amount));
        public static Item Create(ItemNames name) => Create((int)name);
        public static Item Create(int id) => Collections.Items.ContainsKey(id) ? Create(Collections.Items[id]) : CreateInvalidItem();

        public static Item Create(Item original)
        {
            if (!Enum.IsDefined(typeof(RebornItemEffect), original.RebornEffect))
                original.RebornEffect = RebornItemEffect.None;
            var clone = CloneChamber.Clone(original);
            clone.UniqueId = UniqueIdGenerator.GetNext(EntityType.Item);
            return clone;
        }

        private static int IdFromAmount(int amount)
        {
            int id;
            if (amount <= 10 && amount >= 1)
                id = 1090000; //Silver
            else if (amount <= 100 && amount >= 10)
                id = 1090010; //Sycee
            else if (amount <= 1000 && amount >= 100)
                id = 1090020; //Gold
            else if (amount <= 10000 && amount >= 1000)
                id = 1091010; //GoldBar
            else if (amount > 10000)
                id = 1091020; //GoldBars
            else
                id = 0;
            return id;
        }

        internal static Item CreateInvalidItem()
        {
            var item = default(Item);
            item.ItemId = -1;
            return item;
        }
    }
}
