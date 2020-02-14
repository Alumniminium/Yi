using System;
using Yi.Entities;
using Yi.Enums;
using Yi.Items;
using Yi.SelfContainedSystems;

namespace Yi.Calculations
{
    public static class AttackCalcs
    {
        public static int MagicDmg(YiObj attacker, Monster target)
        {
            float damage;
            if (attacker.CurrentSkill.Id == 1115 || attacker.CurrentSkill.Info.WeaponSubType != 0 && attacker.CurrentSkill.Info.WeaponSubType != 500)
            {
                damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 30000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;
                damage = AdjustPvE(damage, attacker, target);

                damage -= target.Defense;
            }
            else if (attacker.CurrentSkill.Info.WeaponSubType == 500)
            {
                damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 30000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;
                damage = AdjustPvE(damage, attacker, target);
            }
            else
            {
                damage = attacker.MagicAttack;
                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 30000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;
                damage = AdjustPvE(damage, attacker, target);

                damage *= (float)(100 - target.MagicDefense) / 100;
                damage -= target.MagicDefense; //target.MagicBlock;
                damage *= 0.75f;
            }

            if (damage < 1)
                damage = 1;

            damage = AdjustMinPvE(damage, attacker);

            attacker.Experience += AdjustExp((int)Math.Round(damage, 0), attacker, target);
            TeamSystem.ShareExp(attacker, target, (uint)Math.Round(target.MaximumHp * 0.05));
            return (int)Math.Round(damage, 0);
        }

        public static int MagicDmg(YiObj attacker, YiObj target)
        {
            float damage;

            var reborn = 1.00f;
            if (target.Reborn)
                reborn -= 0.30f; //30%

            var dodge = 1.00f;
            dodge -= target.Dexterity / 100;

            if (attacker.CurrentSkill.Id == 1115 || attacker.CurrentSkill.Info.WeaponSubType != 0 && attacker.CurrentSkill.Info.WeaponSubType != 500)
            {
                damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                if (attacker.HasFlag(StatusEffect.SuperMan) && target is Player)
                    damage *= 0.2f; //PvP Reduction!

                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 30000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;

                damage -= target.Defense;
            }
            else if (attacker.CurrentSkill.Info.WeaponSubType == 500)
            {
                damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                if (attacker.HasFlag(StatusEffect.SuperMan)&&target is Player)
                    damage *= 0.2f; //PvP Reduction!

                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 30000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;
                
            }
            else
            {
                damage = attacker.MagicAttack;
                if (attacker.CurrentSkill.Info.Power > 30000)
                    damage *= (float)(attacker.CurrentSkill.Info.Power - 3000) / 100;
                else
                    damage += attacker.CurrentSkill.Info.Power;

                damage *= (float)(100 - Math.Min(target.MagicDefense, 95)) / 100;
                damage -= target.MagicDefense; //target.MagicBlock;
                damage *= 0.65f;
            }

            damage *= reborn;

            if (damage < 1)
                damage = 1;

            return (int)Math.Round(damage, 0);
        }

        public static int GetDamage(YiObj attacker, YiObj target, MsgInteractType attackType)
        {
            if (!YiCore.Success(attacker.Dexterity))
                return 0;

            var damage = 1.0d;
            
            switch (attackType)
            {
                case MsgInteractType.Physical:
                    damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                    if (attacker is Monster)
                        damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                    if (target is Monster monster)
                        damage = AdjustPvE((float)damage, attacker, monster);
                    if (attacker is Monster monster1 && target != null)
                        damage = AdjustEvP((float)damage, monster1, target);
                    if (target != null)
                        damage -= target.Defense;
                    break;
                case MsgInteractType.Magic:
                    damage = attacker.MagicAttack;
                    //if (attacker is Monster)
                    //    damage = (attacker as Monster).Base.MagicAttack;

                    //if (attacker is YiObj && target is Monster)
                    //    damage = AdjustPvE(damage, (YiObj)attacker, (Monster)target);
                    //if (attacker is Monster && target is YiObj)
                    //    damage = AdjustEvP(damage, (Monster)attacker, (YiObj)target);
                    if (target != null)
                    {
                        damage *= (float) (100 - Math.Min(target.MagicDefense, 95)) / 100;
                        damage -= target.MagicDefense; //MagicBlock
                        damage *= 0.65;
                    }
                    else
                    {
                        damage *= 0.75;
                    }
                    break;
                case MsgInteractType.Archer:
                    if (attacker == null)
                        return 0;

                    damage = YiCore.Random.Next(attacker.MinimumPhsyicalAttack, attacker.MaximumPhsyicalAttack);
                    damage = AdjustPvE((float) damage, attacker, attacker.CurrentTarget as Monster);
                    break;
            }
            
            if (target != null)
            {
                if (target.Reborn)
                    damage *= 0.7;

                damage *= Math.Max(target.Bless, 1);
            }
            if (damage > 0)
                damage = YiCore.Random.Next((int) (damage / 1.2), (int) damage);

            damage *= 0.75;

            damage = Math.Max(1, damage);

            if (attacker is Player aPlayer)
            {

               if (target == null)
                    return (int) Math.Round(damage, 0);

                if (attacker.HasFlag(StatusEffect.SuperMan))
                    damage *= 10;
            }

            if (target != null)
                TeamSystem.ShareExp(attacker, target, (uint) Math.Round(target.MaximumHp * 0.05));
            attacker.Experience += AdjustExp((int)Math.Round(damage, 0), attacker, target);

            return (int)Math.Round(damage, 0);
        }

        public static uint AdjustExp(int damage, YiObj attacker, YiObj target)
        {
            byte level = 120;
            if (attacker.Level < 120)
                level = attacker.Level;

            float exp = Math.Min(damage, target.CurrentHp);
            var deltaLvl = level - target.Level;

            if (target.IsGreen(attacker))
            {
                if (deltaLvl >= 3 && deltaLvl <= 5)
                    exp *= 0.70f;
                else if (deltaLvl > 5 && deltaLvl <= 10)
                    exp *= 0.20f;
                else if (deltaLvl > 10 && deltaLvl <= 20)
                    exp *= 0.10f;
                else if (deltaLvl > 20)
                    exp *= 0.05f;
            }
            else if (target.IsRed(attacker))
                exp *= 1.30f;
            else if (target.IsBlack(attacker))
            {
                if (deltaLvl >= -10 && deltaLvl < -5)
                    exp *= 1.5f;
                else if (deltaLvl >= -20 && deltaLvl < -10)
                    exp *= 1.8f;
                else if (deltaLvl < -20)
                    exp *= 2.3f;
            }

            double adjustedExp = Math.Max(1, exp);
            if (target is Player)
            {
                if (target.HasFlag(StatusEffect.RedName))
                    adjustedExp = adjustedExp * 0.1;
                else if (target.HasFlag(StatusEffect.BlackName))
                    adjustedExp = adjustedExp * 0.2;
                else
                    adjustedExp = adjustedExp * 0.02;

                if (attacker is Player)
                    target.Experience -= (uint)Math.Max(1, adjustedExp);
            }

            return (uint)Math.Max(1, adjustedExp);
        }

        public static float AdjustMinEvP(float damage, Monster attacker, YiObj target)
        {
            var minDmg = 7;
            if (damage >= minDmg || target.Level <= 15)
                return (int)damage;

            minDmg += attacker.Level / 10;

            if (target.Equipment.TryGetValue(MsgItemPosition.Armor, out var item))
            {
                minDmg -= item.ItemId % 10;

                if (item.ItemId % 10 == 0)
                    minDmg = 1;
            }
            minDmg = Math.Max(1, minDmg);

            return Math.Max(minDmg, (int)damage);
        }

        public static float AdjustEvP(float damage, Monster attacker, YiObj target)
        {
            byte level = 120;
            if (attacker.Level < 120)
                level = attacker.Level;

            if (attacker.IsRed(target))
                damage *= 1.5f;
            else if (attacker.IsBlack(target))
            {
                var deltaLvl = target.Level - level;
                if (deltaLvl >= -10 && deltaLvl <= -5)
                    damage *= 2.0f;
                else if (deltaLvl >= -20 && deltaLvl < -10)
                    damage *= 3.5f;
                else if (deltaLvl < -20)
                    damage *= 5.0f;
            }

            return Math.Max(0, (int)damage);
        }

        public static float AdjustMinPvE(float damage, YiObj attacker)
        {
            var minDmg = 1;
            minDmg += attacker.Level / 10;
            
            if (!attacker.Equipment.TryGetValue(MsgItemPosition.Armor, out var item))
                return Math.Max(minDmg, (int) damage);

            minDmg += item.ItemId % 10;

            if (item.ItemId % 10 == 0)
                minDmg = 1;
            return Math.Max(minDmg, (int)damage);
        }

        public static float AdjustPvE(float damage, YiObj attacker, Monster target)
        {
            if (target == null || attacker==null)
                return 0;
            if (!target.IsGreen(attacker))
                return Math.Max(0, (int)damage);

            var deltaLvl = attacker.Level - target.Level;
            if (deltaLvl >= 3 && deltaLvl <= 5)
                damage *= 1.5f;
            else if (deltaLvl > 5 && deltaLvl <= 10)
                damage *= 2;
            else if (deltaLvl > 10 && deltaLvl <= 20)
                damage *= 2.5f;
            else if (deltaLvl > 20)
                damage *= 3;
            else
                damage *= 1;

            return Math.Max(0, (int)damage);
        }
    }
}