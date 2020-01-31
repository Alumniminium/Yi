using System.Collections.Concurrent;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.World;

namespace YiX.Structures
{
    public class Storage
    {
        public int OwnerId;
        public int StorageId;
        public int NpcId;
        public string AccessCode;
        public string StorageName;
        public int Money;
        public ConcurrentDictionary<int, Item> Contents;
        public ConcurrentDictionary<int, StorageAccess> Members;

        public static Storage Create(YiObj owner, int npcId)
        {
            var storage = new Storage
            {
                OwnerId = owner.UniqueId,
                NpcId = npcId,
                StorageId = SafeRandom.Next(0, 1000),
                AccessCode = "",
                StorageName = "Unnamed Storage",
                Contents = new ConcurrentDictionary<int, Item>(),
                Members = new ConcurrentDictionary<int, StorageAccess>()
            };
            storage.Members.AddOrUpdate(owner.UniqueId, StorageAccess.Owner);
            return storage;
        }

        public bool CanSeeItems(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.ItemSee);
        public bool CanSeeMoney(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.MoneySee);
        public bool CanAddItems(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.AddItems);
        public bool CanAddMoney(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.AddMoney);
        public bool CanTakeItems(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.TakeItems);
        public bool CanTakeMoney(YiObj obj) => Members.ContainsKey(obj.UniqueId) && Members[obj.UniqueId].HasFlag(StorageAccess.TakeMoney);

        public YiObj GetOwner()
        {
            if (!GameWorld.Find(OwnerId, out YiObj owner))
                Output.WriteLine("Owner not found. Shit broken.");
            return owner;
        }
        public string ToString(YiObj req)
        {
            var moneyAccess = "";
            var itemAccess = "";
            if (Members.ContainsKey(req.UniqueId))
            {
                if (Members[req.UniqueId].HasFlag(StorageAccess.MoneyAdd))
                    moneyAccess += "R";
                if (Members[req.UniqueId].HasFlag(StorageAccess.MoneyTake))
                    moneyAccess += "W";
                if (Members[req.UniqueId].HasFlag(StorageAccess.ItemAdd))
                    itemAccess += "R";
                if (Members[req.UniqueId].HasFlag(StorageAccess.ItemTake))
                    itemAccess += "W";
            }
            return $"{StorageId.ToString("00000")}   {StorageName.Size32(' ')}   {GetOwner().Name.Replace("\0", "").Size16(' ')}   Money: {moneyAccess}   Items: {itemAccess}";
        }
    }
}
