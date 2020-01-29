using System.Collections.Generic;
using System.Linq;

namespace YiX.Calculations
{
    public struct Conditions
    {
        public static bool True(IEnumerable<bool> conditions) => !conditions.Contains(false);

        public static bool False(IEnumerable<bool> conditions) => !conditions.Contains(true);
    }
}