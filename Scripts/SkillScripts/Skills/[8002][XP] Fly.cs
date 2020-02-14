using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace SkillScripts.Skills
{
    [Script(Id = (int)SkillId.Fly)]
    public static class Fly
    {
        public static TimeSpan BuffDuration;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(40);
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            //Map map;
            //if (GameWorld.Maps.TryGetValue(target.MapId, out map))
            //{
            //    if (map.Flags.HasFlag(MapFlags.DisableFly))
            //        target.GetMessage("SYSTEM", target.Name, "Flying forbidden on this map.", MsgTextType.Action);
            //        return false;
            //}

            var flyBuff = new Buff(target, SkillId.Fly, BuffDuration)
            {
                Effect = StatusEffect.Flying,
                Description = $"XP Fly:{skill.Level}: Makes you fly :3"
            };
            BuffSystem.AddBuff(target, flyBuff);
            target.RemoveStatusEffect(StatusEffect.XpList);
            return true;
        }
    }
}
