using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Yi.Enums;

namespace Yi.SelfContainedSystems
{
    public static class UniqueIdGenerator
    {
        public static Dictionary<EntityType, int> UniqueIds = new Dictionary<EntityType, int>();

        public static void Load()
        {
            if (File.Exists("Database\\UniqueIds.json"))
            {
                UniqueIds = JsonConvert.DeserializeObject<Dictionary<EntityType, int>>(File.ReadAllText("Database\\UniqueIds.json"));
                Output.WriteLine("UniqueIds: " + UniqueIds.Count);
            }
            else
            {
                Output.WriteLine("Creating new UniqueId file.");
                UniqueIds.Add(EntityType.Player, 1000000);
                UniqueIds.Add(EntityType.Monster, 500000);
                UniqueIds.Add(EntityType.Npc, 0);
                UniqueIds.Add(EntityType.DynamicNpc, 10000);
                UniqueIds.Add(EntityType.Booth, 200000);
                UniqueIds.Add(EntityType.Portal, 10000000);
                UniqueIds.Add(EntityType.FloorItem, 900000);
                UniqueIds.Add(EntityType.Item, 80000);
            }
        }

        public static int GetNext(EntityType type)
        {
            if (!UniqueIds.ContainsKey(type))
                throw new NullReferenceException("No Unique UniqueId entry for " + type);
            UniqueIds[type]++;
            switch (type)
            {
                case EntityType.Item:
                    if (UniqueIds[type] == int.MaxValue)
                        UniqueIds[type] = 80000;
                    break;
                case EntityType.FloorItem:
                    if (UniqueIds[type] == 9999999)
                        UniqueIds[type] = 900000;
                    break;
            }

            return UniqueIds[type];
        }

        public static void Goto(int uniqueId, EntityType npc)
        {
            if (!UniqueIds.ContainsKey(npc))
                throw new NullReferenceException("No Unique UniqueId entry for NPCs");
            UniqueIds[npc] = uniqueId;
        }
    }
}