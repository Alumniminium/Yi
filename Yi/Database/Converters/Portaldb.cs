using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yi.Enums;
using Yi.SelfContainedSystems;
using Yi.World;

namespace Yi.Database.Converters
{
    public static class PortalDb
    {
        public static Task Load()
        {
            return Task.Run(() =>
            {
                foreach (var portalFile in Directory.GetDirectories("RAW\\Portals\\").SelectMany(Directory.GetFiles))
                {
                    using (var reader = new KeyValueFormat(portalFile))
                    {
                        var obj = new Portal
                        {
                            MapId = reader.Load<ushort>("FromMap"),
                            ToMap = reader.Load<ushort>("ToMap"),
                            ToX = reader.Load<ushort>("ToX"),
                            ToY = reader.Load<ushort>("ToY"),
                            UniqueId = UniqueIdGenerator.GetNext(EntityType.Portal)
                        };
                        obj.Location.X = reader.Load<ushort>("FromX");
                        obj.Location.Y = reader.Load<ushort>("FromY");
                        if (obj.MapId == 0)
                            continue;
                        if (GameWorld.Maps.ContainsKey(obj.MapId))
                        {
                            Portal.Portals.Add(obj.UniqueId, obj);
                            //GameWorld.Maps[obj.MapId].LoadInEntity(obj);
                        }
                    }
                }
            });
        }
    }
}