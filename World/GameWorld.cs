using System;
using System.Collections.Generic;
using System.Linq;
using YiX.Entities;
using YiX.Items;

namespace YiX.World
{
    public struct GameWorld
    {
        public static Dictionary<ushort, Map> Maps = new Dictionary<ushort, Map>();
        public static bool Find<T>(int uniqueId, out T obj)
        {
            obj = default(T);
            foreach (var map in Maps.Values)
            {
                if (map.Entities == null)
                    continue;
                if (!map.Entities.TryGetValue(uniqueId, out var found))
                    continue;
                if (found is T)
                    obj = (T) (object) found;
                break;
            }
            return obj != null;
        }
        public static bool Find<T>(string name, out T obj)
        {
            obj = default(T);
            foreach (var map in Maps.Values)
            {
                foreach (var value in map.Entities.Values)
                {
                    var mapObject = value as Player;
                    if (mapObject != null)
                    {
                        var targetName = mapObject.Name.TrimEnd('\0');
                        if (targetName == name)
                        {
                            obj = (T) (object) mapObject;
                            break;
                        }
                    }
                }
            }
            return obj != null;
        }
        public static int CountOnlinePlayers() => Maps.Values.Sum(m => m.Entities.Values.OfType<Player>().Count(p => p.Online));
        public static int CountMonsters() => Maps.Values.Sum(m => m.Entities.Values.OfType<Monster>().Count());
        public static int CountNpcs() => Maps.Values.Sum(m => m.Entities.Values.OfType<Npc>().Count());
        public static int CountDynNpcs() => Maps.Values.Sum(m => m.Entities.Values.OfType<DynamicNpc>().Count());
        public static int CountBots() => Maps.Values.Sum(m => m.Entities.Values.OfType<Bot>().Count());
        public static int CountFloorItems() => Maps.Values.Sum(m => m.Entities.Values.OfType<FloorItem>().Count());
        public static bool NameAvailable(string name) => Maps.Values.All(map => !map.Entities.Values.OfType<Player>().Select(entity => entity).Any(player => string.Equals(player.Name, name, StringComparison.InvariantCultureIgnoreCase)));
    }
}