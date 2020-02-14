using System.Collections.Concurrent;
using Yi.Entities;
using Yi.Helpers;
using Yi.Scheduler;

namespace Yi.SelfContainedSystems
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
