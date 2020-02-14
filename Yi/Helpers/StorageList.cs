using System.Linq;
using Yi.Structures;

namespace Yi.Helpers
{
    public class StorageList :SafeList<Storage>
    {
        public bool TryGetValue(int storageId, out Storage value)
        {
            lock (Items)
            {
                foreach (var entry in this.Where(entry => entry.StorageId == storageId))
                {
                    value = entry;
                    return true;
                }
            }
            value = null;
            return false;
        }
    }
}