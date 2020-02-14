using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Yi.Enums;
using Yi.Items;

namespace Yi.Helpers
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void AddOrUpdate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value)
        {
            dictionary?.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            TValue ignored;
            return self.TryRemove(key, out ignored);
        }

        public static bool ContainsAll(this ConcurrentDictionary<int, Item> dictionary, IEnumerable<int> ids)
        {
            return ids.Select(id => dictionary.Any(item => item.Value.ItemId == id)).All(found => found);
        }

        public static bool ContainsAll(this ConcurrentDictionary<int, Item> dictionary, IEnumerable<ItemNames> ids)
        {
            return ids.Select(id => dictionary.Any(item => item.Value.ItemId == (int)id)).All(found => found);
        }
    }
}