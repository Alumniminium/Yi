using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace SkillScripts.Skills
{

    [Script(Id = (int)SkillId.MagicShield)]
    public static class MagicShield
    {
        public static TimeSpan BuffDuration;
        public static float DefenseBonusPercent;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(30);
            switch (skill.Level)
            {
                case 0:
                    DefenseBonusPercent = 1.1f;
                    break;
                case 1:
                    DefenseBonusPercent = 1.5f;
                    break;
            }
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            var shieldBuff = new Buff(target, SkillId.MagicShield, BuffDuration)
            {
                PhysDefMod = DefenseBonusPercent,
                Effect = StatusEffect.MagicShield,
                Description = $"MagicShield Level {skill.Level}: +{(DefenseBonusPercent * 100) - 100}% Physical Defense."
            };

            BuffSystem.AddBuff(target, shieldBuff);
            return true;
        }
    }
}