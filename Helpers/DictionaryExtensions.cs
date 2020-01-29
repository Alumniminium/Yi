using System.Collections.Generic;
using System.Linq;
using YiX.Enums;
using YiX.Items;

namespace YiX.Helpers
{
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }
            dictionary.Add(key, value);
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, out TValue value)
        {
            if (self.TryGetValue(key, out value))
            {
                self.Remove(key);
                return true;
            }
            return false;
        }

        public static bool ContainsAll(this Dictionary<uint, Item> dictionary, IEnumerable<uint> ids)
        {
            return ids.Select(id => dictionary.Any(item => item.Value.ItemId == id)).All(found => found);
        }

        public static bool ContainsAll(this Dictionary<uint, Item> dictionary, IEnumerable<ItemNames> ids)
        {
            return ids.Select(id => dictionary.Any(item => item.Value.ItemId == (uint)id)).All(found => found);
        }
    }
}