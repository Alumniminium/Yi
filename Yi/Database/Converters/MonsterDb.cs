using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yi.AI;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.Database.Converters
{
    public static class MonsterDb
    {
        public static Task Load()
        {
            return Task.Run(async () =>
            {
                short counter = 0;
                foreach (var mobName in Directory.GetDirectories("RAW\\MonsterSpawns\\").SelectMany(Directory.GetDirectories))
                {
                    for (var i = 0; i < Directory.GetFiles(mobName).Length; i++)
                    {
                        var file = Directory.GetFiles(mobName)[i];
                        using (var reader = new KeyValueFormat(file))
                        {
                            var spawn = new Spawn
                            {
                                MapId = reader.Load<ushort>("mapid"),
                                MobId = reader.Load<uint>("npctype"),
                                Xstart = reader.Load<ushort>("bound_x"),
                                Ystart = reader.Load<ushort>("bound_y"),
                                Xend = reader.Load<short>("bound_cx"),
                                Yend = reader.Load<short>("bound_cy"),
                                RespawnDelay = reader.Load<ushort>("rest_secs"),
                                Amount = reader.Load<ushort>("max_per_gen")
                            };
                            if (!Collections.Spawns.ContainsKey(counter))
                                Collections.Spawns.TryAdd(counter, spawn);
                            counter++;
                        }
                    }
                }
                await LoadMobs();
            });
        }

        private static Task LoadMobs()
        {
            return Task.Run(async () =>
            {
                foreach (var file in Directory.EnumerateFileSystemEntries("RAW\\Monsters\\"))
                {
                    using (var reader = new KeyValueFormat(file))
                    {
                        var name = reader.Load<string>("name");
                        var id = reader.Load<uint>("id");
                        Collections.MonsterNames.AddOrUpdate(id, name);
                        var monster = new Monster
                        {
                            Name = name.Size16(),
                            Id = id,
                            Look = reader.Load<uint>("lookface"),
                            CurrentHp = (ushort)Math.Min(reader.Load<int>("life"), ushort.MaxValue),
                            MaximumHp = (ushort)Math.Min(reader.Load<int>("life"), ushort.MaxValue),
                            MaximumPhsyicalAttack = reader.Load<int>("attack_max"),
                            MinimumPhsyicalAttack = reader.Load<int>("attack_min"),
                            Defense = reader.Load<ushort>("defence"),
                            Dexterity = reader.Load<ushort>("dodge"),
                            AttackRange = reader.Load<byte>("attack_range"),
                            AttackSpeed = reader.Load<int>("attack_speed"),
                            Level = reader.Load<byte>("level"),
                            Drops = new Drops
                            {
                                Money = reader.Load<ushort>("drop_money"),
                                ItemType = (short)reader.Load<int>("drop_itemtype"),
                                Armet = (short)reader.Load<int>("drop_armet"),
                                Necklace = (short)reader.Load<int>("drop_necklace"),
                                Armor = (short)reader.Load<int>("drop_armor"),
                                Ring = (short)reader.Load<int>("drop_ring"),
                                Weapon = (short)reader.Load<int>("drop_weapon"),
                                Shield = (short)reader.Load<int>("drop_shield"),
                                Shoes = (short)reader.Load<int>("drop_shoes"),
                                Hp = reader.Load<int>("drop_hp"),
                                Mp = reader.Load<int>("drop_mp")
                            },
                            MagicType = (short)reader.Load<int>("magic_type"),
                            MagicDefense = reader.Load<int>("magic_def"),
                            MagicHitRate = (short)reader.Load<int>("magic_hitrate"),
                        };
                        monster.MaximumPhsyicalAttack += (short)reader.Load<int>("extra_damage");
                        monster.MinimumPhsyicalAttack += (short)reader.Load<int>("extra_damage");
                        Collections.BaseMonsters.AddOrUpdate(id, monster);
                    }
                }
                await Spawn();
            });
        }

        public static Task Spawn(int max = 0)
        {
            return Task.Run(() =>
            {
                foreach (var spawn in Collections.Spawns)
                {
                    Monster monster;
                    if (!Collections.BaseMonsters.TryGetValue(spawn.Value.MobId, out monster))
                        continue;
                    for (var i = 0; i < spawn.Value.Amount; i++)
                    {

                        var obj = CloneChamber.Clone(Collections.BaseMonsters[spawn.Value.MobId]);
                        obj.SpawnId = spawn.Key;
                        obj.UniqueId = UniqueIdGenerator.GetNext(EntityType.Monster);
                        obj.MapId = spawn.Value.MapId;
                        obj.Location.X = spawn.Value.Xstart;
                        obj.Location.Y = spawn.Value.Ystart;
                        
                        if (!GameWorld.Maps.ContainsKey(obj.MapId)) continue;
                        var spawnAttempts = 0;
                        while (true)
                        {
                            if (spawnAttempts > 1000)
                                break;

                            spawnAttempts++;
                            if (GameWorld.Maps[obj.MapId].MobValid(obj.Location.X, obj.Location.Y))
                            {
                                Collections.Monsters.AddOrUpdate(obj.UniqueId, obj);
                                GameWorld.Maps[obj.MapId].LoadInEntity(obj);
                                break;
                            }

                            obj.Location.X = (ushort)YiCore.Random.Next(spawn.Value.Xstart - 10, spawn.Value.Xstart + spawn.Value.Xend + 10);
                            obj.Location.Y = (ushort)YiCore.Random.Next(spawn.Value.Ystart - 10, spawn.Value.Ystart + spawn.Value.Yend + 10);

                            if (obj.Look == 900 || obj.Look == 910)//Guard1 Guard2
                            {
                                obj.Brain = new GuardBrain(obj);
                            }
                        }
                    }
                }
            });
        }
     }
}