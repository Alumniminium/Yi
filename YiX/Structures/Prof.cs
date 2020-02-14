using System;
using Newtonsoft.Json;

namespace YiX.Structures
{
    [Serializable]
    public struct Prof
    {
        [JsonIgnore]
        public static readonly uint[] ProfExp = {0, 1200, 68000, 250000, 640000, 1600000, 4000000, 10000000, 22000000, 40000000, 90000000, 95000000, 142500000, 213750000, 320625000, 480937500, 721406250, 1082109375, 1623164063, 2100000000};

        public uint Experience { get; set; }
        public ushort Id { get; set; }
        public byte Level { get; set; }

        public Prof(ushort id, byte level, uint experience)
        {
            Experience = experience;
            Level = level;
            Id = id;
        }
    }
}