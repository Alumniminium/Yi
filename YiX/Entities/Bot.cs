using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using YiX.Enums;
using YiX.Structures;
using YiX.World;

namespace YiX.Entities
{
    [Serializable]
    public class Bot : YiObj
    {
        [JsonIgnore]
        private static Dictionary<uint, Bot> _bots = new Dictionary<uint, Bot>();
        [JsonIgnore]
        public static Dictionary<uint, Bot> Bots
        {
            get { return _bots; }
            set
            {
                _bots = value;
                foreach (var kvp in value)
                    GameWorld.Maps[kvp.Value.MapId].LoadInEntity(kvp.Value);
            }
        }

        public Bot()
        {
            Profs = new Dictionary<int, Prof>();
            Skills = new Dictionary<SkillId, Skill>();
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
        }

        public override string ToString() => Name;
    }
}