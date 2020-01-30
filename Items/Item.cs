using System;
using YiX.Database.Squiggly.Models;
using YiX.Enums;

namespace YiX.Items
{
    [Serializable]
    public struct Item
    {
        public int UniqueId { get; set; }
        public int OwnerUniqueId { get; set; }
        public int ItemId { get; set; }
        public byte Level { get; set; }
        public ushort CurrentDurability { get; set; }
        public ushort MaximumDurability { get; set; }
        public short MaximumPhsyicalAttack { get; set; }
        public short MinimumPhsyicalAttack { get; set; }
        public short MagicAttack { get; set; }
        public short MagicDefense { get; set; }
        public short Defense { get; set; }
        public ushort Agility { get; set; }
        public byte Range { get; set; }
        public ushort Frequency { get; set; }
        public int PriceBaseline { get; set; }
        public byte Prof { get; set; }
        public short ReqStr { get; set; }
        public short ReqSpi { get; set; }
        public short ReqAgi { get; set; }
        public short ReqVit { get; set; }
        public byte Sex { get; set; }
        public ushort PotAddHp { get; set; }
        public ushort PotAddMp { get; set; }
        public byte StackAmount
        {
            get => (byte)(CustomTextId % 10000000);
            set
            {
                var amount = CustomTextId % 10000000;
                CustomTextId -= amount;
                amount = value;
                CustomTextId += amount;
            }
        }
        public int BonusId => GetBonusId();
        public byte Plus { get; set; }
        public byte Bless { get; set; }
        public byte Enchant { get; set; }
        public byte Gem1 { get; set; }
        public byte Gem2 { get; set; }
        public RebornItemEffect RebornEffect { get; set; }
        public int CustomTextId { get; set; }
        public cq_itemaddition Bonus { get; set; }
        public short Dodge { get; set; }

        private int GetBonusId()
        {
            int bonusId;

            if (ItemId > 110000 && ItemId < 120000) //Head
                bonusId = ItemId - ItemId % 10 - (ItemId % 1000 - ItemId % 100);
            else if (ItemId > 130000 && ItemId < 140000) //Armor
                bonusId = ItemId - ItemId % 10 - (ItemId % 1000 - ItemId % 100);
            else if (ItemId > 400000 && ItemId < 500000 && !(ItemId > 421000 && ItemId < 422000)) //Weapon (1H)
                bonusId = 444000 + (ItemId % 1000 - ItemId % 10);
            else if (ItemId > 500000 && ItemId < 600000 && !(ItemId > 500000 && ItemId < 501000)) //Weapon (2H)
                bonusId = 555000 + (ItemId % 1000 - ItemId % 10);
            else if (ItemId > 900000 && ItemId < 1000000) //Shield
                bonusId = ItemId - ItemId % 10 - (ItemId % 1000 - ItemId % 100);
            else
                bonusId = ItemId - ItemId % 10;

            bonusId *= 100;
            bonusId += Plus;

            return bonusId;
        }

        public override string ToString() => $"UniqueId: {UniqueId} Id: {ItemId} Plus:{Plus} Bless:{Bless} Enchant{Enchant} Gems:{Gem1},{Gem2} RebornId:{RebornEffect} Restrain:{CustomTextId} " + base.ToString();

        internal bool Valid() => ItemId != -1;
    }
}