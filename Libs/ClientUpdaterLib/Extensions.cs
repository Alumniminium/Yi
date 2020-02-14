using System.Collections.Concurrent;

namespace ClientUpdaterLib
{
    public static class Extensions
    {
        public static void AddOrUpdate<TKey, TVal>(this ConcurrentDictionary<TKey, TVal> dict, TKey key, TVal val)
        {
            if (dict.ContainsKey(key))
                dict[key] = val;
            else
                dict.TryAdd(key,val);
        }
    }
}
