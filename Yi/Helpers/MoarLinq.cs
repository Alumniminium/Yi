using System;
using System.Collections.Generic;

namespace Yi.Helpers
{
    public static class MoarLinq
    {
        public static IEnumerable<TSource> TakeUpTo<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return TakeIterator(source, count);
        }

        private static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            foreach (var element in source)
            {
                yield return element;
                if (--count == 0)
                    break;
            }
        }
    }
}
