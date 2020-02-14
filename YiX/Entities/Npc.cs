using System;
using YiX.Enums;
using YiX.Structures;

namespace YiX.Entities
{
    [Serializable]
    public class Npc : YiObj
    {
        public byte Type { get; set; }
        public override Direction Direction
        {
            get => (Direction)(Type % 10);
        }
        public byte Base { get; set; }
        public byte Sort { get; set; }
        public long Task0 { get; set; }
        public long Task1 { get; set; }
        public long Task2 { get; set; }
        public long Task3 { get; set; }
        public long Task4 { get; set; }
        public long Task5 { get; set; }
        public long Task6 { get; set; }
        public long Task7 { get; set; }

        public Npc()
        {
            Inventory = new Inventory(this);
            BoothId = UniqueId;
        }
        public override string ToString() => UniqueId.ToString();
    }
}