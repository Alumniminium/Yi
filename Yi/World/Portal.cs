using System.Collections.Generic;
using Newtonsoft.Json;
using Yi.Entities;

namespace Yi.World
{
    public class Portal : YiObj
    {
        [JsonIgnore]
        private static Dictionary<int, Portal> _portals = new Dictionary<int, Portal>();
        [JsonIgnore]
        public static Dictionary<int, Portal> Portals
        {
            get => _portals;
            set
            {
                _portals = value;
                foreach (var kvp in value)
                    GameWorld.Maps[kvp.Value.MapId].LoadInEntity(kvp.Value);
            }
        }
        public ushort ToY { get; set; }
        public ushort ToX { get; set; }
        public ushort ToMap { get; set; }
    }
}