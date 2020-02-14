using System.Collections.Concurrent;
using System.Collections.Generic;
using YiX.Database.Squiggly.Models;
using YiX.Entities;
using YiX.Items;
using YiX.Structures;

namespace YiX.Database
{
    public static class Collections
    {
        public static Dictionary<int, Portal> Portals = new Dictionary<int, Portal>();
        public static Dictionary<int, Statpoint> Statpoints = new Dictionary<int, Statpoint>();
        public static ConcurrentDictionary<short, Spawn> Spawns = new ConcurrentDictionary<short, Spawn>();
        public static Dictionary<int, MagicTypeEntry> Skills = new Dictionary<int, MagicTypeEntry>();
        public static Dictionary<int, LevelExp> LevelExps = new Dictionary<int, LevelExp>();
        public static Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public static Dictionary<int, cq_itemaddition> ItemBonus = new Dictionary<int, cq_itemaddition>();
        public static Dictionary<int, Monster> BaseMonsters = new Dictionary<int, Monster>();
        public static Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        public static Dictionary<int, string> MonsterNames = new Dictionary<int, string>();
        public static Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();
        public static Dictionary<int, YiObj> Npcs = new Dictionary<int, YiObj>();
    }
}