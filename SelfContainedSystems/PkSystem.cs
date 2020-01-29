using System.Collections.Concurrent;
using YiX.Entities;
using YiX.Helpers;
using YiX.Scheduler;

namespace YiX.SelfContainedSystems
{
    public static class PkSystem
    {
        private static ConcurrentDictionary<Player, Job> Jobs = new ConcurrentDictionary<Player, Job>();
        public static void CreateFor(Player player)
        {
            Jobs.AddOrUpdate(player, null);
        }

        private static void Work()
        {
            while (true)
            {
                
            }
        }
    }
}
