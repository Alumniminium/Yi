using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace SkillScripts.Skills
{

    [Script(Id = (int)SkillId.Shield)]
    public static class Shield
    {
        public static TimeSpan BuffDuration;
        public static float DefenseBonusPercent;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(120);
            switch (skill.Level)
            {
                case 0:
                    DefenseBonusPercent = 1.5f;
                    break;
            }
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            var shieldBuff = new Buff(target, SkillId.Shield, BuffDuration)
            {
                PhysDefMod = DefenseBonusPercent,
                Effect = StatusEffect.MagicShield,
                Description = $"MagicShield Level {skill.Level}: +{(DefenseBonusPercent*100)-100}% Physical Defense."
            };
            target.RemoveStatusEffect(StatusEffect.XpList);
            BuffSystem.AddBuff(target, shieldBuff);
            return true;
        }
    }
}