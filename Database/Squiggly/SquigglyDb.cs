using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YiX.AI;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Database.Squiggly
{
    public static class SquigglyDb
    {
        public static void Spawn()
        {
            var _sync = new object();
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(Collections.Spawns, spawn =>
            {
                var amount = spawn.Value.Amount;

                if (!Collections.BaseMonsters.TryGetValue(spawn.Value.MobId, out var monster))
                    return;

                if (monster.Look != 900 && monster.Look != 910)
                    amount = (ushort) (amount * 9);

                for (var i = 0; i < amount; i++)
                {
                    var obj = CloneChamber.Clone(Collections.BaseMonsters[spawn.Value.MobId]);
                    obj.SpawnId = spawn.Key;
                    obj.MapId = spawn.Value.MapId;
                    obj.Location.X = spawn.Value.Xstart;
                    obj.Location.Y = spawn.Value.Ystart;

                    var spawnAttempts = 0;
                    while (true)
                    {
                        if (spawnAttempts > 100)
                            break;

                        spawnAttempts++;
                        lock (_sync)
                        {
                            if (!GameWorld.Maps.ContainsKey(obj.MapId) || GameWorld.Maps[obj.MapId].MobValid(obj.Location.X, obj.Location.Y))
                            {
                                if (obj.Look == 900 || obj.Look == 910) //Guard1 Guard2
                                {
                                    obj.Brain = new GuardBrain(obj);
                                }

                                obj.UniqueId = UniqueIdGenerator.GetNext(EntityType.Monster);
                                Collections.Monsters.Add(obj.UniqueId, obj);
                                GameWorld.Maps[obj.MapId].LoadInEntity(obj);
                                break;
                            }
                        }

                    obj.Location.X = (ushort) YiCore.Random.Next(spawn.Value.Xstart - 10,
                            spawn.Value.Xstart + spawn.Value.Xend + 10);
                        obj.Location.Y = (ushort) YiCore.Random.Next(spawn.Value.Ystart - 10,
                            spawn.Value.Ystart + spawn.Value.Yend + 10);
                    }
                }
            });
            sw.Stop();
            Debug.WriteLine($"[MobGenerator] Spawnd {Collections.Monsters.Count}\t Monsters in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadMaps()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqmap in db.cq_map.AsQueryable())
                {
                    var map = new Map
                    {
                        Id = (ushort)cqmap.id,
                        MapDocId = (ushort)cqmap.mapdoc,
                        Flags = (MapFlags)cqmap.type,
                        Name = cqmap.name.Trim(),
                        RespawnLocation = new Tuple<ushort, ushort, ushort>((ushort)cqmap.portal0_x, (ushort)cqmap.portal0_y, (ushort)cqmap.reborn_map),
                        Width = cqmap.Width,
                        Height = cqmap.Height,
                    };
                    if (cqmap.DMAP != null)
                        MapAccess.MapData.AddOrUpdate((ushort) cqmap.mapdoc, new MapAccess(cqmap.DMAP));
                    GameWorld.Maps.Add((ushort)cqmap.id, map);
                }
                foreach (var dportal in db.Dmap_Portals)
                {
                    if (GameWorld.Maps.ContainsKey(dportal.MapId))
                    {
                        var passageInfo = new Portal
                        {
                            IdX = dportal.PortalId,
                            X = dportal.X,
                            Y = dportal.Y
                        };

                        GameWorld.Maps[dportal.MapId].Portals.Add(dportal.PortalId, passageInfo);
                    }
                }
            }
            
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {GameWorld.Maps.Count}\t Maps in {sw.Elapsed.TotalMilliseconds}ms");
        }
        public static void LoadNpcs()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqNpc in db.cq_npc.AsQueryable())
                {
                    if (!GameWorld.Maps.ContainsKey(cqNpc.mapid))
                        continue;
                    var npc = new Npc
                    {
                        UniqueId = (int) cqNpc.id,
                        Location =
                        {
                            X = cqNpc.cellx,
                            Y = cqNpc.celly
                        },
                        MapId = cqNpc.mapid,
                        Sort = cqNpc.sort,
                        Base = cqNpc.@base,
                        Type = cqNpc.type,
                        Look = cqNpc.lookface,
                        Name = cqNpc.name.Trim(),
                        Task0 = cqNpc.task0,
                        Task1 = cqNpc.task1,
                        Task2 = cqNpc.task2,
                        Task3 = cqNpc.task3,
                        Task4 = cqNpc.task4,
                        Task5 = cqNpc.task5,
                        Task6 = cqNpc.task6,
                        Task7 = cqNpc.task7, 
                    };

                    Collections.Npcs.Add(npc.UniqueId, npc);

                    GameWorld.Maps[npc.MapId].LoadInEntity(npc);
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.Npcs.Count}\t Npcs in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadMobs()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqMob in db.cq_monstertype.AsQueryable())
                {
                    var mob = new Monster
                    {
                        Id = cqMob.id,
                        Name = cqMob.name.Trim(),
                        Look = cqMob.lookface,
                        MaximumHp = cqMob.life,
                        CurrentHp = cqMob.life,
                        MaximumPhsyicalAttack = cqMob.attack_max,
                        MinimumPhsyicalAttack = cqMob.attack_min,
                        Defense = cqMob.defence,
                        Dexterity = cqMob.dexterity,
                        Dodge = cqMob.dodge,
                        Drops = new Drops
                        {
                            Armet = cqMob.drop_armet,
                            Armor = cqMob.drop_armor,
                            Weapon = cqMob.drop_weapon,
                            Hp = cqMob.drop_hp,
                            Mp = cqMob.drop_mp,
                            ItemType = cqMob.drop_itemtype,
                            Money = cqMob.drop_money,
                            Necklace = cqMob.drop_necklace,
                            Ring = cqMob.drop_ring,
                            Shield = cqMob.drop_shield,
                            Shoes = cqMob.drop_shoes
                        },
                        AttackRange = cqMob.attack_range,
                        ViewRange = cqMob.view_range,
                        EscapeLife = cqMob.escape_life,
                        AttackSpeed = cqMob.attack_speed,
                        WalkSpeed = cqMob.move_speed,
                        RunSpeed = cqMob.run_speed,
                        Level = cqMob.level,
                        AttackUser = cqMob.attack_user,
                        CQAction = cqMob.action,
                        MagicType = cqMob.magic_type,
                        MagicDefense = cqMob.magic_def,
                        MagicHitRate = cqMob.magic_hitrate,
                        AIType = cqMob.ai_type,
                    };
                    if (Collections.BaseMonsters.ContainsKey(mob.Id))
                    {
                        continue;
                    }
                    Collections.BaseMonsters.Add(mob.Id, mob);
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.BaseMonsters.Count}\t BaseMonsters in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadSpawns()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                short counter = 0;
                foreach (var cqSpawn in db.cq_generator.AsQueryable())
                {
                    var spawn = new Spawn
                    {
                        MapId = cqSpawn.mapid,
                        MobId = cqSpawn.npctype,
                        BornX = cqSpawn.born_x,
                        BornY = cqSpawn.born_y,
                        TimerBegin =  cqSpawn.timer_begin,
                        TimerEnd =  cqSpawn.timer_end,
                        MaxAmount =  cqSpawn.maxnpc,
                        Xstart =  cqSpawn.bound_x,
                        Ystart =  cqSpawn.bound_y,
                        Xend =  cqSpawn.bound_cx,
                        Yend =  cqSpawn.bound_cy,
                        RespawnDelay =  cqSpawn.rest_secs,
                        Amount =  cqSpawn.max_per_gen
                    };
                    if (!Collections.Spawns.ContainsKey(counter))
                        Collections.Spawns.TryAdd(counter, spawn);
                    counter++;
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.Spawns.Count}\t Spawns in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadLevelExp()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqLvlExp in db.cq_levexp.AsQueryable())
                {
                    var lvlExp = new LevelExp
                    {
                        AllLevTime = cqLvlExp.up_lev_time,
                        ExpReq = cqLvlExp.exp
                    };
                    Collections.LevelExps.Add((int) cqLvlExp.level, lvlExp);
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.LevelExps.Count}\t LevelExps in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadPortals()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqPortal in db.cq_portal.AsQueryable())
                {
                    var portal = new Portal
                    {
                        MapId = cqPortal.mapid,
                        X = cqPortal.portal_x,
                        Y = cqPortal.portal_y,
                        Id = cqPortal.id,
                        IdX = cqPortal.portal_idx
                    };
                    Collections.Portals.Add(portal.Id, portal);
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.Portals.Count}\t Portals in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public static void LoadItemBonus()
        {
            var sw = Stopwatch.StartNew();
            using (var db = new SquigglyContext())
            {
                foreach (var cqItemAdd in db.cq_itemaddition.AsQueryable())
                {
                    Collections.ItemBonus.Add(cqItemAdd.id, cqItemAdd);
                }
            }
            sw.Stop();
            Debug.WriteLine($"[SquigglyLite] Loaded {Collections.ItemBonus.Count}\t Item Bonus Stats in {sw.Elapsed.TotalMilliseconds}ms");
        }
    }
}