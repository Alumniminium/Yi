using System.Collections.Concurrent;
using System.Collections.Generic;
using Yi.Entities;
using Yi.Items;
using Yi.Structures;
using Yi.World;

namespace Yi.Database
{
    public static class Collections
    {
        public static Dictionary<int, Statpoint> Statpoints = new Dictionary<int, Statpoint>();
        public static ConcurrentDictionary<short, Spawn> Spawns = new ConcurrentDictionary<short, Spawn>();
        public static Dictionary<int, MagicTypeEntry> Skills = new Dictionary<int, MagicTypeEntry>();
        public static Dictionary<int, LevelExp> LevelExps = new Dictionary<int, LevelExp>();
        public static Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public static Dictionary<int, ItemBonus> ItemBonus = new Dictionary<int, ItemBonus>();
        public static Dictionary<uint, Monster> BaseMonsters = new Dictionary<uint, Monster>();
        public static readonly Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        public static Dictionary<uint, string> MonsterNames = new Dictionary<uint, string>();
        private static Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        public static Dictionary<int, Monster> Monsters
        {
            get => _monsters;
            set
            {
                _monsters = value;
                foreach (var kvp in value)
                {
                    GameWorld.Maps[kvp.Value.MapId].LoadInEntity(kvp.Value);
                    if (!kvp.Value.Alive)
                        kvp.Value.CurrentHp = kvp.Value.MaximumHp;
                }
            }
        }
        private static Dictionary<int, YiObj> _npcs = new Dictionary<int, YiObj>();
        public static Dictionary<int, YiObj> Npcs
        {
            get => _npcs;
            set
            {
                _npcs = value;
                foreach (var kvp in value)
                    GameWorld.Maps[kvp.Value.MapId].LoadInEntity(kvp.Value);
            }
        }
    }
}