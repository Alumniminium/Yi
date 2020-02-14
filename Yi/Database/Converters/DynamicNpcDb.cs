using System.IO;
using Yi.Entities;
using Yi.Enums;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Database.Converters
{
    public static class DynamicNpcDb
    {
        public static void Load()
        {
                var count = 100000;
                foreach (var file in Directory.EnumerateFileSystemEntries("RAW\\DynNpcs\\"))
                {
                    using (var reader = new KeyValueFormat(file))
                    {
                        var map = reader.Load<int>("Map");
                        if (map > 5000)
                            continue;

                        var obj = new DynamicNpc
                        {
                            UniqueId = count,
                            Type = reader.Load<byte>("Action"),
                            Look = reader.Load<uint>("Look"),
                            MapId = reader.Load<ushort>("Map"),
                            MaximumHp = reader.Load<ushort>("HP"),
                            CurrentHp = reader.Load<ushort>("HP"),
                            Base = reader.Load<byte>("Base"),
                            Sort = reader.Load<byte>("Sort"),
                            Defense = reader.Load<ushort>("Defense"),
                            MagicDefense = reader.Load<ushort>("MDefense")};

                    UniqueIdGenerator.Goto(obj.UniqueId, EntityType.DynamicNpc);
                    obj.Location.X = reader.Load<ushort>("X");
                        obj.Location.Y = reader.Load<ushort>("Y");

                    if (obj.MapId == 0)
                            continue;
                        if (GameWorld.Maps.ContainsKey(obj.MapId))
                        {
                            Collections.Npcs.Add(obj.UniqueId, obj);
                            GameWorld.Maps[obj.MapId].LoadInEntity(obj);
                        }
                        count++;
                    }
                }
                ShopDb.LoadShops();
        }
    }
}