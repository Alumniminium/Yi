using System;
using System.Collections.Generic;
using YiX.Entities;
using YiX.Items;

namespace YiX.Structures
{
    [Serializable]
    public class Trade
    {
        public readonly YiObj Owner, Partner;
        public readonly Dictionary<int, Item> OwnerItems, PartnerItems;
        public int OwnerMoney, PartnerMoney;
        public int OwnerCps, PartnerCps;
        public bool OwnerOk, PartnerOk;

        public Trade(YiObj player, YiObj target)
        {
            Owner = player;
            Partner = target;
            OwnerItems = new Dictionary<int, Item>();
            PartnerItems = new Dictionary<int, Item>();
        }
    }
}