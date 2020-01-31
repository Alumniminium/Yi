using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Packets.Conquer;

namespace YiX.Structures
{
    [Serializable]
    [DataContract]
    public class Inventory
    {
        [JsonIgnore]
        private YiObj _owner;
        [DataMember]
        public ConcurrentDictionary<int, Item> Items;
        public int Count => Items.Count;

        public Inventory(YiObj owner)
        {
            _owner = owner;
            Items = new ConcurrentDictionary<int, Item>();
        }

        public void SetOwner(YiObj owner) => _owner = owner;

        public bool FindByUID(int uniqueId, out Item found) => Items.TryGetValue(uniqueId, out found);
        public bool RemoveItem(ItemNames name, int count = 1) => RemoveItem(name, count);
        private bool RemoveItem(int id, int count = 1)
        {
            if (HasItem(id, count))
            {
                for (var i = 0; i < count; i++)
                {
                    foreach (var item in Items.Values.Where(item => item.ItemId == id))
                    {
                        Items.TryRemove(item.UniqueId);
                        (_owner as Player)?.Send(MsgItem.Create(item.UniqueId, 0, 0, MsgItemType.RemoveInventory));
                    }
                }
                return true;
            }
            return false;
        }

        public void RemoveItem(Item remove)
        {
            (_owner as Player)?.Send(MsgItem.Create(remove.UniqueId, 0, 0, MsgItemType.RemoveInventory));
            Items.TryRemove(remove.UniqueId);
        }

        public void AddBypass(Item item) => Items.TryAdd(item.UniqueId, item);

        public bool AddItem(Item item, int count = 1)
        {
            if (Items.Count + count > 40)
                return false;

            for (var i = 0; i < count; i++)
            {
                var clone = CloneChamber.Clone(item);
                clone.UniqueId = YiCore.Random.Next(1000, 100000);
                Items.TryAdd(clone.UniqueId, clone);
                (_owner as Player)?.Send(new MsgItemInformation(clone, MsgItemPosition.Inventory));
            }
            return true;
        }
        public bool RemoveItems(params ItemNames[] itemIds) => Items.ContainsAll(itemIds) && itemIds.All(itemId => RemoveItem(itemId));
        internal bool RemoveItems(params int[] itemIds) => Items.ContainsAll(itemIds) && itemIds.All(itemId => RemoveItem(itemId));
        public bool HasItem(ItemNames name, int count = 1) => HasItem((int)name, count);
        internal bool HasItem(int id, int count = 1) => Items.Values.Count(i => i.ItemId == id) >= count;
        internal bool HasItems(params int[] itemIds) => Items.ContainsAll(itemIds);
        public bool HasItems(params ItemNames[] itemIds) => Items.ContainsAll(itemIds);

        public IEnumerator<KeyValuePair<int, Item>> GetEnumerator() => Items.GetEnumerator();

        public void AddOrUpdate(int uniqueId, Item original) => Items.AddOrUpdate(uniqueId, original);

        public bool TryRemove(int uniqueId, out Item item) => Items.TryRemove(uniqueId, out item);
    }
}
