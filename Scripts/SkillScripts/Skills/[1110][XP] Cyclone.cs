using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace SkillScripts.Skills
{

    [Script(Id = (int)SkillId.Cyclone)]
    public static class Cyclone
    {
        public static TimeSpan BuffDuration;
        public static float SpeedBonusPercent;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(25);
            switch (skill.Level)
            {
                case 0:
                    SpeedBonusPercent = 2.0f;
                    break;
            }
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            var shieldBuff = new Buff(target, SkillId.Cyclone, BuffDuration)
            {
                SpeedMod = SpeedBonusPercent,
                Effect = StatusEffect.Cyclone,
                Description = "Cyclone: +100% Speed!"
            };
            target.RemoveStatusEffect(StatusEffect.XpList);
            BuffSystem.AddBuff(target, shieldBuff);
            return true;
        }
    }
}