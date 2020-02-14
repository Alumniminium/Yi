using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yi.AI;
using Yi.Database.Converters;
using Yi.Database.Converters.Dmap;
using Yi.Entities;
using Yi.Enums;
using Yi.Helpers;
using Yi.Items;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.Database
{
    public static class Db
    {
        public static readonly List<Task> TaskList = new List<Task>();
        
        public static async Task Load()
        {
            //if (!Directory.Exists("RAW"))
              //  await Content.Content.RestoreAsync();

            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");
            await TryLoadAsync();
            YiCore.ScheduleFullBackup();
            YiCore.ScheduleDynamicBackup();
        }

        public static async Task SaveAsJsonAsync(SaveType saveType)
        {
            Output.WriteLine("Saving Database!");
            
            if (saveType == SaveType.Dynamic || saveType == SaveType.All)
            {
                TaskList.Add(ChatLog.Save());
                Serialize(nameof(MobDropSystem.Drops), MobDropSystem.Drops);
                Serialize(nameof(GameWorld.Maps), GameWorld.Maps);
                Serialize(nameof(FloorItemSystem.FloorItems), FloorItemSystem.FloorItems);
                Serialize(nameof(Portal.Portals), Portal.Portals);
                Serialize(nameof(Collections.Npcs), Collections.Npcs);
                Serialize(nameof(Collections.BaseMonsters), Collections.BaseMonsters);
                Serialize(nameof(Collections.Monsters), Collections.Monsters);
                Serialize(nameof(Bot.Bots), Bot.Bots);
                Serialize(nameof(BoothSystem.BoothPool), BoothSystem.BoothPool);
                Serialize(nameof(StorageSystem.StoragePool), StorageSystem.StoragePool);
                Serialize(nameof(UniqueIdGenerator.UniqueIds), UniqueIdGenerator.UniqueIds);
                Serialize(nameof(SelectorSystem.Players), SelectorSystem.Players);
            }
            if (saveType == SaveType.Static || saveType == SaveType.All)
            {
                Serialize(nameof(Collections.LevelExps), Collections.LevelExps);
                Serialize(nameof(Collections.Statpoints), Collections.Statpoints);
                Serialize(nameof(Collections.Skills), Collections.Skills);
                Serialize(nameof(Collections.ItemBonus), Collections.ItemBonus);
                Serialize(nameof(Collections.Items), Collections.Items);
                Serialize(nameof(Collections.Spawns), Collections.Spawns);
                SerializeMaps();
            }

            await Task.WhenAll(TaskList);
            TaskList.Clear();

            YiCore.CompactLoh();
            Output.WriteLine("Saved Database!");
        }

        private static void SerializeMaps()
        {
            TaskList.Add(Task.Run(() =>
            {
                using (var stream = new FileStream("Database\\MapData.nano", FileMode.Create))
                {
                    var dserializer = new DataContractSerializer(MapAccess.MapData.GetType());
                    dserializer.WriteObject(stream, MapAccess.MapData);
                    stream.Flush();
                    stream.Close();
                }
            }));
        }

        public static void Serialize(string propertyPath, object obj)
        {
            TaskList.Add(Task.Run(() =>
            {
                var serializer = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    Converters = { new BoolConverter() }
                };
                using (var stream = new StreamWriter($"Database//{propertyPath}.json"))
                    serializer.Serialize(stream, obj, obj.GetType());
            }));
        }


        private static async Task<bool> TryLoadAsync()
        {
            try
            {
                UniqueIdGenerator.Load();
                LoadMaps();
                LoadItems();
                await Task.WhenAll(LoadItemBonus(), 
                    LoadLevelExp(), LoadStatpoints(), LoadSkills(),
                    LoadMobs(), LoadAccounts(), LoadBots(), 
                    LoadNpcs(), LoadFloorItems(), LoadPortals(), LoadStoragePool(),LoadBoothPool(), LoadMobDrops());

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
                Output.WriteLine("|---> Portals:      " + Portal.Portals.Count);
                Output.WriteLine("|---> Mob Drops:    " + MobDropSystem.Drops.Count);
                SetUpScriptingEngine();
                Output.WriteLine("|---> Script Count: " + ScriptEngine.Scripts.Count);
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

        private static Task LoadMobDrops()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                try
                {
                    using (var stream = new StreamReader("Database\\Drops.json"))
                    using (var reader = new JsonTextReader(stream))
                        MobDropSystem.Drops= serializer.Deserialize<ConcurrentDictionary<int, Drop>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    Output.WriteLine("MOB DROP SYSTEM GONE!");
                }
            });
        }

        private static Task LoadStoragePool()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                try
                {
                    using (var stream = new StreamReader("Database\\StoragePool.json"))
                    using (var reader = new JsonTextReader(stream))
                        StorageSystem.StoragePool = serializer.Deserialize<ConcurrentDictionary<int, StorageList>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    Output.WriteLine("STORAGE SYSTEM GONE!");
                }
            });
        }

        private static Task LoadBoothPool()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                try
                {
                    using (var stream = new StreamReader("Database\\BoothPool.json"))
                    using (var reader = new JsonTextReader(stream))
                        BoothSystem.BoothPool =
                            serializer.Deserialize<ConcurrentDictionary<int, ConcurrentDictionary<int, Item>>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    Output.WriteLine("BOOTH SYSTEM GONE!");
                }
            });
        }

        private static void LoadMaps()
        {
            var serializer = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                Converters = {new BoolConverter()}
            };
            try
            {
                using (var stream = new StreamReader("Database\\Maps.json"))
                using (var reader = new JsonTextReader(stream))
                    GameWorld.Maps = serializer.Deserialize<Dictionary<ushort, Map>>(reader);

                using (var stream = new FileStream("Database\\MapData.nano", FileMode.Open))
                {
                    var dserializer = new DataContractSerializer(MapAccess.MapData.GetType());
                    MapAccess.MapData = (Dictionary<ushort, MapAccess>) dserializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
                Output.WriteLine("Rebuilding maps...");
                MapManager.Load(@"RAW\ini\GameMap.dat", Environment.CurrentDirectory + "\\RAW\\");
                using (var stream = new StreamWriter("Database\\Maps.json"))
                    serializer.Serialize(stream, GameWorld.Maps, GameWorld.Maps.GetType());

                using (var stream = new FileStream("Database\\MapData.nano", FileMode.Create))
                {
                    var dserializer = new DataContractSerializer(MapAccess.MapData.GetType());
                    dserializer.WriteObject(stream, MapAccess.MapData);
                    stream.Flush();
                    stream.Close();
                }
                LoadMaps();
            }
        }

        private static Task LoadBots()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Bots.json"))
                    using (var reader = new JsonTextReader(stream))
                        Bot.Bots = serializer.Deserialize<Dictionary<uint, Bot>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    Output.WriteLine("Ignoring Bots...");
                    Bot.Bots = new Dictionary<uint, Bot>();
                }
            });
        }

        private static Task LoadMobs()
        {
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Spawns.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.Spawns = serializer.Deserialize<ConcurrentDictionary<short, Spawn>>(reader);
                    using (var stream = new StreamReader("Database\\BaseMonsters.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.BaseMonsters = serializer.Deserialize<Dictionary<uint, Monster>>(reader);
                    using (var stream = new StreamReader("Database\\Monsters.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.Monsters = serializer.Deserialize<Dictionary<int, Monster>>(reader);
                    using (var stream = new StreamReader("Database\\MonsterNames.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.MonsterNames = serializer.Deserialize<Dictionary<uint, string>>(reader);

                    foreach (var kvp in Collections.Monsters)
                    {
                        var mob = kvp.Value;
                        if (mob.Look == 900 || mob.Look == 910)
                            mob.Brain = new GuardBrain(mob);
                        else
                            mob.Brain = new BasicBrain(mob);
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await MonsterDb.Load();
                    using (var stream = new StreamWriter("Database\\BaseMonsters.json"))
                        serializer.Serialize(stream, Collections.BaseMonsters, Collections.BaseMonsters.GetType());
                    using (var stream = new StreamWriter("Database\\Monsters.json"))
                        serializer.Serialize(stream, Collections.Monsters, Collections.Monsters.GetType());
                    using (var stream = new StreamWriter("Database\\Spawns.json"))
                        serializer.Serialize(stream, Collections.Spawns, Collections.Spawns.GetType());
                    using (var stream = new StreamWriter("Database\\MonsterNames.json"))
                        serializer.Serialize(stream, Collections.MonsterNames, Collections.MonsterNames.GetType());
                }
            });
        }

        private static Task LoadNpcs()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Npcs.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.Npcs = serializer.Deserialize<Dictionary<int, YiObj>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    NpcDb.Load();
                    using (var stream = new StreamWriter("Database\\Npcs.json"))
                        serializer.Serialize(stream, Collections.Npcs, Collections.Npcs.GetType());
                }
            });
        }

        private static void LoadItems()
        {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
            try
            {
                using (var stream = new StreamReader("Database\\Items.json"))
                using (var reader = new JsonTextReader(stream))
                    Collections.Items = serializer.Deserialize<Dictionary<int, Item>>(reader);
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
                Item.ItemFactory.LoadDb();
                using (var stream = new StreamWriter("Database\\Items.json"))
                    serializer.Serialize(stream, Collections.Items, Collections.Items.GetType());
            }
        }

        private static Task LoadItemBonus()
        {
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\ItemBonus.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.ItemBonus = serializer.Deserialize<Dictionary<int, ItemBonus>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await ItemBonusConverter.Load();
                    using (var stream = new StreamWriter("Database\\ItemBonus.json"))
                        serializer.Serialize(stream, Collections.ItemBonus, Collections.ItemBonus.GetType());
                }
            });
        }

        private static Task LoadPortals()
        {
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Portals.json"))
                    using (var reader = new JsonTextReader(stream))
                        Portal.Portals = serializer.Deserialize<Dictionary<int, Portal>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await PortalDb.Load();
                    using (var stream = new StreamWriter("Database\\Portals.json"))
                        serializer.Serialize(stream, Portal.Portals, Portal.Portals.GetType());
                }
            });
        }

        private static Task LoadFloorItems()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\FloorItems.json"))
                    using (var reader = new JsonTextReader(stream))
                        FloorItemSystem.FloorItems = serializer.Deserialize<ConcurrentDictionary<int, FloorItem>>(reader);
                    
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    // ignored
                }
            });
        }

        private static Task LoadSkills()
        {
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Skills.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.Skills = serializer.Deserialize<Dictionary<int, MagicTypeEntry>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await MagicTypeConverter.Load();
                    using (var stream = new StreamWriter("Database\\Skills.json"))
                        serializer.Serialize(stream, Collections.Skills, Collections.Skills.GetType());
                }
            });
        }

        private static Task LoadStatpoints()
        {
            //StatpointConverter.Load();
            //return Task.Delay(1);
            
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Statpoints.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.Statpoints = serializer.Deserialize<Dictionary<int, Statpoint>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await StatpointConverter.Load();
                    using (var stream = new StreamWriter("Database\\Statpoints.json"))
                        serializer.Serialize(stream, Collections.Statpoints, Collections.Statpoints.GetType());
                }
            });
        }

        private static Task LoadAccounts()
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\Players.json"))
                    using (var reader = new JsonTextReader(stream))
                        SelectorSystem.Players = serializer.Deserialize<ConcurrentDictionary<string, List<Player>>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    Output.WriteLine("OH FUCK. ACCOUNTS GONE!");
                }
            });
        }

        private static Task LoadLevelExp()
        {
            return Task.Run(async () =>
            {
                var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, Converters = {new BoolConverter()}};
                try
                {
                    using (var stream = new StreamReader("Database\\LevelExps.json"))
                    using (var reader = new JsonTextReader(stream))
                        Collections.LevelExps = serializer.Deserialize<Dictionary<int, LevelExp>>(reader);
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex);
                    await LevelExpConverter.Load();
                    using (var stream = new StreamWriter("Database\\LevelExps.json"))
                        serializer.Serialize(stream, Collections.LevelExps, Collections.LevelExps.GetType());
                }
            });
        }

        public static void SetUpScriptingEngine()
        {
            if (Directory.Exists("Temp"))
                Directory.Delete("Temp", true);
            Directory.CreateDirectory("Temp");
            foreach (var entry in Directory.EnumerateDirectories(@"Scripts\"))
            {
                var scriptName = Path.GetFileNameWithoutExtension(entry);
                var script = new ScriptContainer(scriptName);
                if (script.Compile())
                    ScriptEngine.Scripts.Add(script.ScriptType, script);
            }
        }
    }
}