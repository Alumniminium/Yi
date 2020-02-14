using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace SkillScripts.Skills
{
    [Script(Id = (int)SkillId.Invisibility)]
    public static class Invisibility
    {
        public static TimeSpan BuffDuration;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(120);
            switch (skill.Level)
            {
                case 0:
                    BuffDuration = TimeSpan.FromSeconds(30);
                    break;
                case 1:
                    BuffDuration = TimeSpan.FromSeconds(60);
                    break;
                case 2:
                    BuffDuration = TimeSpan.FromSeconds(90);
                    break;
                case 3:
                    BuffDuration = TimeSpan.FromSeconds(120);
                    break;
            }
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            var shieldBuff = new Buff(target, SkillId.Shield, BuffDuration)
            {
                Effect = StatusEffect.Invisibility,
                Description = $"Invisibility Level {skill.Level}"
            };
            target.RemoveStatusEffect(StatusEffect.XpList);
            BuffSystem.AddBuff(target, shieldBuff);
            return true;
        }
    }
}