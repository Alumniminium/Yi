using System;
using Yi.Structures;

namespace Yi.Entities
{
    [Serializable]
    public class Npc : YiObj
    {
        public byte Type { get; set; }
        public byte Base { get; set; }
        public byte Sort { get; set; }

        public Npc()
        {
            Inventory = new Inventory(this);
            BoothId = UniqueId;

        }
        public override string ToString() => UniqueId.ToString();
    }
}