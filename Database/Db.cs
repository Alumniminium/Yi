using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YiX.Database.Squiggly;
using YiX.Entities;
using YiX.Helpers;
using YiX.Items;
using YiX.Scripting;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Database
{
    public static class Db
    {
        public static string DbRoot = Environment.CurrentDirectory;
        public static JsonSerializer serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        public static readonly List<Task> TaskList = new List<Task>();

        public static async Task<bool> Load()
        {
            try
            {
                UniqueIdGenerator.Load();
                LoadItems();
                SquigglyDb.LoadMaps();
                SquigglyDb.LoadMobs();
                SquigglyDb.LoadSpawns();
                SquigglyDb.LoadNpcs();
                SquigglyDb.LoadLevelExp();
                SquigglyDb.LoadPortals();
                SquigglyDb.LoadItemBonus();
                SquigglyDb.Spawn();

                await Task.WhenAll(LoadStatpoints(), LoadSkills(), LoadAccounts(),
                    LoadFloorItems(), LoadStoragePool(), LoadBoothPool());


                StorageSystem.SetUpStorageSpaces();
                BoothSystem.SetUpBooths();
                FloorItemSystem.SetUpFloorItemSystem();

                Output.WriteLine("|------------ Player Data ------------");
                Output.WriteLine("|");
                Output.WriteLine("|---> Accounts:     " + SelectorSystem.Players.Count);
                Output.WriteLine("|---> Storages:     " + StorageSystem.StoragePool.Count);
                Output.WriteLine("|---> Booths:       " + BoothSystem.BoothPool.Count);
                Output.WriteLine("|");
                Output.WriteLine("|------------ Common Data ------------");
                Output.WriteLine("|");
                Output.WriteLine("|---> Bots:         " + GameWorld.CountBots());
                Output.WriteLine("|---> Monsters:     " + GameWorld.CountMonsters());
                Output.WriteLine("|---> Npcs:         " + GameWorld.CountNpcs());
                Output.WriteLine("|---> DynamicNpcs:  " + GameWorld.CountDynNpcs());
                Output.WriteLine("|---> Maps:         " + GameWorld.Maps.Count);
                Output.WriteLine("|---> FloorItems:   " + FloorItemSystem.FloorItems.Count);
                Output.WriteLine("|---> Items:        " + Collections.Items.Count);
                Output.WriteLine("|---> ItemBonus:    " + Collections.ItemBonus.Count);
                Output.WriteLine("|---> LevelExp:     " + Collections.LevelExps.Count);
                Output.WriteLine("|---> StatPoints:   " + Collections.Statpoints.Count);
                Output.WriteLine("|---> Skills:       " + Collections.Skills.Count);
                Output.WriteLine("|---> Portals:      " + Collections.Portals.Count);
                Output.WriteLine("|---> Mob Drops:    " + MobDropSystem.Drops.Count);
                await SetUpScriptingEngine();
                Output.WriteLine("|-------------------------------------");
                Output.WriteLine("");
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                YiCore.CompactLoh();
                return true;
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
                return false;
            }
        }

        public static void Serialize(string propertyPath, object obj)
        {
            TaskList.Add(Task.Run(() =>
            {
                using (var stream = new StreamWriter($"Database//{propertyPath}.json"))
                    serializer.Serialize(stream, obj, obj.GetType());
            }));
        }
        private static Task LoadStoragePool()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var stream = new StreamReader(DbRoot + "/Database/StoragePool.json"))
                    using (var reader = new JsonTextReader(stream))
                        StorageSystem.StoragePool = serializer.Deserialize<ConcurrentDictionary<int, StorageList>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                }
            });
        }

        private static Task LoadBoothPool()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var stream = new StreamReader(DbRoot + "/Database/BoothPool.json"))
                    using (var reader = new JsonTextReader(stream))
                        BoothSystem.BoothPool =
                            serializer.Deserialize<ConcurrentDictionary<int, ConcurrentDictionary<int, Product>>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                }
            });
        }

        private static void LoadItems()
        {
            try
            {
                using (var stream = new StreamReader(DbRoot + "/Database/Items.json"))
                using (var reader = new JsonTextReader(stream))
                    Collections.Items = serializer.Deserialize<Dictionary<int, Item>>(reader);
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
            }
        }

        private static Task LoadFloorItems()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var stream = new StreamReader(DbRoot + "/Database/FloorItems.json"))
                    using (var reader = new JsonTextReader(stream))
                        FloorItemSystem.FloorItems = serializer.Deserialize<ConcurrentDictionary<int, FloorItem>>(reader);

                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                }
            });
        }

        private static Task LoadSkills()
        {
            return Task.Run(() =>
           {
               try
               {
                   using (var stream = new StreamReader(DbRoot + "/Database/Skills.json"))
                   using (var reader = new JsonTextReader(stream))
                       Collections.Skills = serializer.Deserialize<Dictionary<int, MagicTypeEntry>>(reader);
               }
               catch (Exception ex)
               {
                   Output.WriteLine(ex);
               }
           });
        }

        private static Task LoadStatpoints()
        {
            return Task.Run(() =>
           {
               try
               {
                   using (var stream = new StreamReader(DbRoot + "/Database/Statpoints.json"))
                   using (var reader = new JsonTextReader(stream))
                       Collections.Statpoints = serializer.Deserialize<Dictionary<int, Statpoint>>(reader);
               }
               catch (Exception ex)
               {
                   Output.WriteLine(ex);
               }
           });
        }

        private static Task LoadAccounts()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var stream = new StreamReader(DbRoot + "/Database/Players.json"))
                    using (var reader = new JsonTextReader(stream))
                        SelectorSystem.Players = serializer.Deserialize<ConcurrentDictionary<string, List<Player>>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                }
            });
        }

        public static async Task SetUpScriptingEngine()
        {
            await ScriptEngine.Configure();

            foreach (var npcScript in Directory.EnumerateFiles(DbRoot + "/NpcScripts"))
            {
                if (!ScriptEngine.CompileNpc(npcScript))
                    Console.WriteLine("[FAILED] " + npcScript);
            }
        }
    }
}