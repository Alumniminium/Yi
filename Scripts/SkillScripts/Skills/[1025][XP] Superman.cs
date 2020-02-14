using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Scripting;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace SkillScripts.Skills
{

    [Script(Id = (int)SkillId.Superman)]
    public static class Superman
    {
        public static TimeSpan BuffDuration;
        public static float AttackBonusPercent;

        public static bool Execute(YiObj target, Skill skill, bool addSkill)
        {
            BuffDuration = TimeSpan.FromSeconds(25);
            switch (skill.Level)
            {
                case 0:
                    AttackBonusPercent = 2.0f;
                    break;
            }
            return addSkill ? AddBuff(target, skill) : RemoveBuff(target);
        }

        private static bool RemoveBuff(YiObj target) => false;

        private static bool AddBuff(YiObj target, Skill skill)
        {
            var shieldBuff = new Buff(target, SkillId.Superman, BuffDuration)
            {
                PhysAtkMod = AttackBonusPercent,
                Effect = StatusEffect.SuperMan,
                Description = "Superman: +1.000% Attack!"
            };
            target.RemoveStatusEffect(StatusEffect.XpList);
            BuffSystem.AddBuff(target, shieldBuff);
            return true;
        }
    }
}