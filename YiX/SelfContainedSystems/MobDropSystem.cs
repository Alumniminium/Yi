using System.Collections.Concurrent;
using YiX.Database;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Structures;
using YiX.World;

namespace YiX.SelfContainedSystems
{
    public static class MobDropSystem
    {
        public static ConcurrentDictionary<int, Drop> Drops = new ConcurrentDictionary<int, Drop>();

        private static readonly ushort[] NecklaceType = { 120, 121 };//120 necklace 121 bags
        private static readonly ushort[] RingType = { 150, 151, 152 };//150 att 151 agi 152 bracelets
        private static readonly ushort[] ArmetType = { 111, 112, 113, 114, 117, 118 };
        private static readonly ushort[] ArmorType = { 130, 131, 132, 133, 134 };
        private static readonly ushort[] OneHanderType = { 410, 420, 421, 430, 440, 450, 460, 480, 481, 490, 500 };//601 was here, but doesn't seem to be valid?
        private static readonly ushort[] TwoHanderType = { 510, 530, 560, 561, 580, 900 };

        private static float GetRates(ushort mapId) => GameWorld.Maps[mapId].DropModifier;

        public static void Drop(YiObj attacker, Monster mob)
        {
            var rand = SafeRandom.Next(1, 1000);

            Item created = default(Item);
            switch (mob.Id)
            {
                case 3131:
                case 3135:
                    created = ItemFactory.Create(ItemNames.ExpPotion); break;//exp potion from titan and gano
                default:
                    {
                        if (!GenerateDrop(attacker, mob, rand, ref created))//YOU FORGOT TO ADD THE ! SO NOTHING WAS DROPPING
                            return;
                        break;
                    }
            }

            if (created.Valid())
                FloorItemSystem.Drop(attacker, mob, created);

            if (YiCore.Success(5)) //5% chance to drop more than one item.
                Drop(attacker, mob);
        }

        private static bool GenerateDrop(YiObj attacker, Monster mob, int rand, ref Item created)
        {
            if (YiCore.Success(50) && Drops.TryGetValue(mob.UniqueId, out var dropList))
            {
                foreach (var item in dropList.Items)
                {
                    if (attacker.Level < item.ReqLevel)
                        continue;

                    switch (WeatherSystem.CurrentWeatherType)
                    {
                        case WeatherType.Rain:
                            {
                                if (!YiCore.Success(item.RainChance))
                                    break;

                                created = ItemFactory.Create(item.ItemId);
                                return true;
                            }
                        case WeatherType.Snow:
                            {
                                if (!YiCore.Success(item.SnowChance))
                                    break;

                                created = ItemFactory.Create(item.ItemId);
                                return true;
                            }
                    }

                    if (DayNightSystem.IsDay() && YiCore.Success(item.DayChance))
                    {
                        created = ItemFactory.Create(item.ItemId);
                        return true;
                    }
                    if (DayNightSystem.IsNight() && YiCore.Success(item.NightChance))
                    {
                        created = ItemFactory.Create(item.ItemId);
                        return true;
                    }
                }
            }

            if (rand < 200)
                FloorItemSystem.DropMoney(attacker, mob, mob.Drops.Money + mob.Level * SafeRandom.Next(1, 10));
            else if (rand < 250)
                created = ItemFactory.Create(mob.Drops.Hp); //HP POTS
            else if (rand < 300)
                created = ItemFactory.Create(mob.Drops.Mp); //MP POTS
            else if (rand < 550) //REGULAR ITEMS
            {
                created = ItemFactory.Create(GenerateItemId(mob));

                if (!created.Valid())
                    return false;

                var plus = GeneratePurity(mob.Level);
                var bless = GenerateBless();

                if (plus == 0)
                    created.Bless = bless;
                if (bless == 0)
                    created.Plus = plus;

                var sockets = GenerateSocketCount(created.ItemId, mob.Level);

                if (sockets != 0)
                    Output.WriteLine($"Dropped {sockets} item.");

                switch (sockets)
                {
                    case 1:
                        created.Gem1 = 255;
                        break;
                    case 2:
                        created.Gem1 = 255;
                        created.Gem2 = 255;
                        break;
                }
            }
            return created.Valid();
        }

        private static int GenerateItemId(Monster mob)
        {
            var itemQuality = GenerateQuality(mob.Level);
            uint itemType = 0;
            short itemLevel;
            bool IsArmet = false, IsArmor = false, IsBracelet = false, IsBag = false;
            var rand = SafeRandom.Next(0, 1200);

            if (mob.Drops.Shoes != 99 && rand >= 0 && rand < 20)
            {
                itemType = 160;
                itemLevel = mob.Drops.Shoes;//??
            }
            else if (mob.Drops.Necklace != 99 && rand >= 20 && rand < 50)
            {
                itemType = NecklaceType[SafeRandom.Next(0, NecklaceType.Length)];
                itemLevel = mob.Drops.Necklace;//??
                IsBag = true;
            }
            else if (mob.Drops.Ring != 99 && rand >= 50 && rand < 100)
            {
                itemType = RingType[SafeRandom.Next(0, RingType.Length)];
                itemLevel = mob.Drops.Ring;//??
                if (itemType == 152) IsBracelet = true;//bracelets are just one level under for some reason?
            }
            else if (mob.Drops.Armet != 99 && rand >= 100 && rand < 400)
            {
                itemType = ArmetType[SafeRandom.Next(0, ArmetType.Length)];
                itemLevel = mob.Drops.Armet;//??
                IsArmet = true;
            }
            else if (mob.Drops.Armor != 99 && rand >= 400 && rand < 700)
            {
                itemType = ArmorType[SafeRandom.Next(0, ArmorType.Length)];
                if (mob.Level > 10 && itemType == 132) itemType++;//noob coats don't work for high level mobs
                itemLevel = mob.Drops.Armor;
                IsArmor = true;
            }
            else // 45%
            {
                var nRate = SafeRandom.Next(0, 100);
                itemLevel = mob.Drops.Weapon;//THIS IS UNIVERSAL AND WASN'T IN THE BACKSWORD SECTION BTW
                if (nRate >= 0 && nRate < 20)
                    itemType = 421;
                else if (mob.Drops.Weapon != 99 && nRate >= 40 && nRate < 80)
                    itemType = OneHanderType[SafeRandom.Next(0, OneHanderType.Length)];
                else if (mob.Drops.Weapon != 99)
                    itemType = TwoHanderType[SafeRandom.Next(0, TwoHanderType.Length)];
            }
            if (itemLevel == 99 || itemLevel == 0 || itemType == 0)
                return 0;

            var itemId = (int)(itemType * 1000 + itemLevel * 10 + itemQuality);

            if (IsArmet || itemType == 900) itemId += 300;
            if (IsArmor) itemId += SafeRandom.Next(0, 9) * 100;//shields might need to be in the armor section?
            if (IsBracelet) itemId -= 90;
            if (IsBag) itemId += 10;

            if (!Collections.Items.ContainsKey(itemId))
                Output.WriteLine("Cody's Mob Drop System generated an invalid ID (AGAIN!): " + itemId);

            return Collections.Items.ContainsKey(itemId) ? itemId : 0;
        }

        private static byte GeneratePurity(int moblevel) => (byte)(SafeRandom.Next(1, 500) <= 3 + moblevel / 25 ? 1 : 0);

        private static byte GenerateBless()
        {
            var rand = SafeRandom.Next(0, 1000);
            if (rand < 1) return 5;
            return (byte)(rand < 5 ? 3 : 0);
        }

        private static byte GenerateSocketCount(int itemId, int moblevel)
        {
            if (itemId < 410000 || itemId > 601999)
                return 0;

            var nRate = SafeRandom.Next(0, 200 - moblevel / 2);

            if (nRate < 5)
                return 2;

            return (byte)(nRate < 20 ? 1 : 0);
        }

        private static byte GenerateQuality(int moblevel)
        {
            var i = SafeRandom.Next(1, 1000);
            i -= moblevel / 25;

            if (i < 1)
                return 9;//super
            if (i < 11)
                return 8;//eli
            if (i < 31)
                return 7;//uni

            return (byte)(i < 55 ? 6 : 3);
        }
    }
}
