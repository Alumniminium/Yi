using System;
using Newtonsoft.Json;
using YiX.Database;
using YiX.Enums;
using YiX.Helpers;
using YiX.SelfContainedSystems;

namespace YiX.Items
{
    public partial class Item
    {
        [JsonIgnore]
        private static ItemFactory _factory;
        [JsonIgnore]
        public static ItemFactory Factory => _factory ?? (_factory = new ItemFactory());
        public class ItemFactory
        {
            public Items.Item CreateMoney(int amount) => Create(IdFromAmount(amount));
            public Items.Item Create(ItemNames name) => Create((int)name);
            internal Items.Item Create(int id) => Collections.Items.ContainsKey(id) ? Create(Collections.Items[id]) : null;

            internal Items.Item Create(Items.Item original)
            {
                if (!Enum.IsDefined(typeof (RebornItemEffect), original.RebornEffect))
                    original.RebornEffect = RebornItemEffect.None;
                var clone = CloneChamber.Clone(original);
                clone.UniqueId = UniqueIdGenerator.GetNext(EntityType.Item);
                return clone;
            }

            private int IdFromAmount(int amount)
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
            /*public static void LoadDb()
            {
                foreach (var file in Directory.EnumerateFileSystemEntries("RAW\\Items\\"))
                {
                    using (var reader = new KeyValueFormat(file))
                    {
                        var item = new Items.Item
                        {
                            ItemId = reader.Load<int>("ItemID"),
                            Level = reader.Load<byte>("ReqLvl"),
                            Prof = reader.Load<byte>("ReqProfLvl"),
                            Sex = reader.Load<byte>("ReqSex"),
                            Dodge = reader.Load<short>("Dodge"),
                            ReqStr = reader.Load<short>("ReqStr"),
                            ReqSpi = reader.Load<short>("ReqSpi"),
                            ReqAgi = reader.Load<short>("ReqAgi"),
                            ReqVit = reader.Load<short>("ReqVit"),
                            PriceBaseline = reader.Load<int>("ShopBuyPrice"),
                            MaximumPhsyicalAttack = reader.Load<short>("MaxPhysAtk"),
                            MinimumPhsyicalAttack = reader.Load<short>("MinPhysAtk"),
                            Defense = reader.Load<short>("PhysDefence"),
                            Agility = reader.Load<ushort>("Dexterity"),
                            PotAddHp = reader.Load<ushort>("PotAddHP"),
                            PotAddMp = reader.Load<ushort>("PotAddMP"),
                            MaximumDurability = reader.Load<ushort>("Durability"),
                            CurrentDurability = reader.Load<ushort>("Arrows"),
                            MagicAttack = reader.Load<short>("MAttack"),
                            MagicDefense = reader.Load<short>("MDefence"),
                            Range = reader.Load<byte>("Range"),
                            Frequency = reader.Load<ushort>("Frequency"),
                            StackAmount = 1
                        };
                        Collections.Items.AddOrUpdate(item.ItemId, item);
                    }
                }
            }*/
        }
    }
}
