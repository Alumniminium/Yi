using System;
using System.Collections.Generic;

namespace YiX.Structures
{
    [Serializable]
    public struct Drop
    {
        public List<DropItem> Items;
    }

    public class DropItem
    {
        public int ItemId;
        public float DayChance = 10;
        public float NightChance = 10;
        public float RainChance = 10;
        public float SnowChance = 10;
        public float ReqLevel = 1;
        public float ActiveQuest = 0;
    }
}
