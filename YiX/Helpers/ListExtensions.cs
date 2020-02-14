using System.Collections.Generic;
using System.Linq;
using YiX.Items;

namespace YiX.Helpers
{
    public static class ListExtensions
    {
        public static bool ContainsAll(this IEnumerable<Item> list, params int[] ids)
        {
            return ids.Select(id => list.Any(item => item.ItemId == id)).All(found => found);
        }
    }
}