using System.IO;
using Yi.Entities;
using Yi.Enums;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Database.Converters
{
    public class NpcDb
    {
        public static void Load()
        {
                Collections.Npcs.Clear();
                foreach (var file in Directory.EnumerateFileSystemEntries("RAW\\Npcs\\"))
                {
                    using (var reader = new KeyValueFormat(file))
                    {
                        var obj = new Npc
                        {
                            UniqueId = reader.Load<int>("id"),
                            Type = reader.Load<byte>("type"),
                            Look = reader.Load<uint>("lookface"),
                            MapId = reader.Load<ushort>("mapid"),
                            Base = reader.Load<byte>("base"),
                            Sort = reader.Load<byte>("sort")
                        };
                        UniqueIdGenerator.Goto(obj.UniqueId, EntityType.Npc);
                        obj.Location.X = reader.Load<ushort>("cellx");
                        obj.Location.Y = reader.Load<ushort>("celly");
                        if (obj.MapId == 0)
                            continue;
                        if (GameWorld.Maps.ContainsKey(obj.MapId))
                        {
                            Collections.Npcs.Add(obj.UniqueId, obj);
                            GameWorld.Maps[obj.MapId].LoadInEntity(obj);
                        }
                    }
                }
                DynamicNpcDb.Load();
        }
    }
}