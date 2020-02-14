using System;

namespace YiX.Entities
{
    [Serializable]
    public class DynamicNpc : YiObj
    {
        public byte Type { get; set; }
        public byte Base { get; set; }
        public byte Sort { get; set; }
    }
}