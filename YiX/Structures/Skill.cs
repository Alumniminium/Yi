using System;
using Newtonsoft.Json;
using YiX.Database;

namespace YiX.Structures
{
    [Serializable]
    public struct Skill
    {
        [JsonIgnore]
        public MagicTypeEntry Info => Collections.Skills.ContainsKey(Id * 10 + Level) ? Collections.Skills[Id * 10 + Level] : new MagicTypeEntry();

        public uint Experience { get; set; }
        public ushort Id { get; set; }
        public byte Level { get; set; }

        public Skill(ushort id, byte level, uint experience)
        {
            Experience = experience;
            Level = level;
            Id = id;
        }
    }
}