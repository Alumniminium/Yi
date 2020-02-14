using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Structures;
using YiX.World;

namespace YiX.SelfContainedSystems
{
    public static class BuffSystem
    {
        public static object _Lock = new Object();
        public static readonly ConcurrentDictionary<int, ConcurrentDictionary<SkillId,Buff>> Entries = new ConcurrentDictionary<int, ConcurrentDictionary<SkillId, Buff>>();
        public static readonly Thread HudThread = new Thread(DisplayHud);

        private static void DisplayHud()
        {
            while (true)
            {
                foreach (var kvp in Entries)
                {
                    if (GameWorld.Find(kvp.Key, out YiObj obj))
                    {
                        if (!(obj is Player))
                            continue;
                        obj.GetMessage("", obj.Name, "------------Active buffs------------", MsgTextType.MiniMap);
                        try
                        {
                            foreach (var buff in kvp.Value.Values)
                            {
                                obj.GetMessage("", obj.Name, buff.ToString(), MsgTextType.MiniMap2);
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                            Output.WriteLine(e);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static void Create(YiObj owner)
        {
            lock (_Lock)
            {
                if (!HudThread.IsAlive)
                {
                    HudThread.IsBackground = true;
                    HudThread.Start();
                }

                if (!Entries.ContainsKey(owner.UniqueId))
                    Entries.TryAdd(owner.UniqueId, new ConcurrentDictionary<SkillId, Buff>());
            }
        }

        public static List<Buff> GetBuffs(YiObj owner)
        {
            Create(owner);
            return (List<Buff>) Entries[owner.UniqueId].Values;
        }

        public static void AddBuff(YiObj owner, Buff buff)
        {
            Create(owner);

            foreach (var oldBuff in Entries[owner.UniqueId].Values)
            {
                if (oldBuff.Description != buff.Description)
                    continue;

                oldBuff.RemoveJob.Cancelled = true;
                Entries[owner.UniqueId].AddOrUpdate(buff.SkillId, buff);

                owner.AddStatusEffect(buff.Effect);
                return;
            }

            Entries[owner.UniqueId].TryAdd(buff.SkillId,buff);
            owner.AddStatusEffect(buff.Effect);
        }

        public static void RemoveBuff(YiObj owner, Buff buff)
        {
            Create(owner);
            Entries[owner.UniqueId].TryRemove(buff.SkillId);
            
            if (Entries[owner.UniqueId].Values.All(b => b.Effect != buff.Effect))
                owner.RemoveStatusEffect(buff.Effect);

            if ((buff.SkillId == SkillId.Superman || buff.SkillId == SkillId.Cyclone) && owner is Player player)
                    player.CurrentKO = 0;
        }

        public static void Clear(YiObj owner)
        {
            if (!Entries.ContainsKey(owner.UniqueId))
                Create(owner);
            foreach (var buff in Entries[owner.UniqueId])
            {
                RemoveBuff(owner, buff.Value);
            }
            Entries[owner.UniqueId].Clear();
        }
    }
}
