using System;
using System.Linq;
using YiX.Database;
using YiX.Entities;
using YiX.Enums;

namespace YiX.Calculations
{
    public struct EntityLogic
    {
        public static void Recalculate(YiObj entity)
        {
            if (entity == null || entity.UniqueId == 0)
                return;
            var defense = 0;
            var dodge = 0;
            var magicDefense = 0;
            var magicAttack = 0;
            var maximumHp = 0;
            var maximumMp = 0;
            var job = entity.Class;
            if (job == 100)
                job += 20;

            if (Collections.Statpoints.ContainsKey(job / 10 * 100 + entity.Level) && entity.Reborn == 0)
            {
                var agility = (ushort)Collections.Statpoints[job / 10 * 100 + entity.Level].Agility;
                var spirit = (ushort)Collections.Statpoints[job / 10 * 100 + entity.Level].Spirit;
                var strength = (ushort)Collections.Statpoints[job / 10 * 100 + entity.Level].Strength;
                var vitality = (ushort)Collections.Statpoints[job / 10 * 100 + entity.Level].Vitality;
                entity.Agility = agility;
                entity.Spirit = spirit;
                entity.Vitality = vitality;
                entity.Strength = strength;
            }

            int minimumPhsyicalAttack = entity.Strength;
            int maximumPhsyicalAttack = entity.Strength;

            maximumHp += entity.Strength * 3;
            maximumHp += entity.Agility * 3;
            maximumHp += entity.Vitality * 24;
            maximumHp += entity.Spirit * 3;

            maximumMp += (ushort)(entity.Spirit * 5);
            if (entity.Class > 100 && entity.Class < 200)
                maximumMp += (ushort)(maximumMp * (entity.Class % 10));

            switch (entity.Class)
            {
                case 11:
                    maximumHp = (int)(maximumHp * 1.05);
                    break;
                case 12:
                    maximumHp = (int)(maximumHp * 1.08);
                    break;
                case 13:
                    maximumHp = (int)(maximumHp * 1.10);
                    break;
                case 14:
                    maximumHp = (int)(maximumHp * 1.12);
                    break;
                case 15:
                    maximumHp = (int)(maximumHp * 1.15);
                    break;
                case 41:
                    entity.AttackSpeed = (int)(entity.AttackSpeed * 0.85);
                    break;
                case 42:
                    entity.AttackSpeed = (int)(entity.AttackSpeed * 0.75);
                    break;
                case 43:
                    entity.AttackSpeed = (int)(entity.AttackSpeed * 0.65);
                    break;
                case 44:
                    entity.AttackSpeed = (int)(entity.AttackSpeed * 0.55);
                    break;
                case 45:
                    entity.AttackSpeed = (int)(entity.AttackSpeed * 0.45);
                    break;
            }

            foreach (var item in entity.Equipment)
            {
                minimumPhsyicalAttack += item.Value.MinimumPhsyicalAttack;
                maximumPhsyicalAttack += item.Value.MaximumPhsyicalAttack;
                defense += item.Value.Defense;
                magicDefense += item.Value.MagicDefense;
                magicAttack += item.Value.MagicAttack;
                maximumHp += item.Value.Enchant;
                maximumHp += item.Value.PotAddHp;
                maximumMp += item.Value.PotAddMp;
                entity.Bless -= item.Value.Bless; //TODO Doublecheck. Tq is weird.
                dodge += item.Value.Dodge;
                entity.AttackRange += item.Value.Range;
                if (item.Value.Frequency > 0)
                    entity.AttackSpeed += 1000 - item.Value.Frequency;
                if (item.Key == MsgItemPosition.RightWeapon || item.Key == MsgItemPosition.LeftWeapon)
                {
                    var subType = item.Value.ItemId / 1000;
                    var sum = (from skill in entity.Skills where skill.Value.Info.WeaponSubType == subType where skill.Value.Level > 12 && skill.Value.Level <= 20 select (20.00f - skill.Value.Level) / 100).Sum();
                    minimumPhsyicalAttack += (int)sum;
                    maximumPhsyicalAttack += (int)sum;
                }
                switch ((ItemNames)item.Value.Gem1)
                {
                    case ItemNames.ShortNormalPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.05f);
                        break;
                    case ItemNames.ShortRefPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.1f);
                        break;
                    case ItemNames.ShortSuperPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.15f);
                        break;
                    case ItemNames.ShortNormalDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.05f);
                        break;
                    case ItemNames.ShortRefDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.1f);
                        break;
                    case ItemNames.ShortSuperDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.15f);
                        break;
                    case ItemNames.ShortNormalFuryGem:
                        dodge += (int)(dodge * 0.05f);
                        break;
                    case ItemNames.ShortRefFuryGem:
                        dodge += (int)(dodge * 0.1f);
                        break;
                    case ItemNames.ShortSuperFuryGem:
                        dodge += (int)(dodge * 0.15f);
                        break;
                    case ItemNames.ShortNormalRainbowGem:
                        entity.ExpBonus += 0.10f;
                        break;
                    case ItemNames.ShortRefRainbowGem:
                        entity.ExpBonus += 0.15f;
                        break;
                    case ItemNames.ShortSuperRainbowGem:
                        entity.ExpBonus += 0.25f;
                        break;
                    case ItemNames.ShortNormalVioletGem:
                        entity.WeaponExpBonus += 0.30f;
                        break;
                    case ItemNames.ShortRefVioletGem:
                        entity.WeaponExpBonus += 0.50f;
                        break;
                    case ItemNames.ShortSuperVioletGem:
                        entity.WeaponExpBonus += 1.00f;
                        break;
                    case ItemNames.ShortNormalMoonGem:
                        entity.MagicExpBonus += 0.30f;
                        break;
                    case ItemNames.ShortRefMoonGem:
                        entity.MagicExpBonus += 0.50f;
                        break;
                    case ItemNames.ShortSuperMoonGem:
                        entity.MagicExpBonus += 1.00f;
                        break;
                    case ItemNames.ShortNormalTortoiseGem:
                        defense -= (int)(defense * 0.02f);
                        break;
                    case ItemNames.ShortRefTortoiseGem:
                        defense -= (int)(defense * 0.04f);
                        break;
                    case ItemNames.ShortSuperTortoiseGem:
                        defense -= (int)(defense * 0.06f);
                        break;
                }
                switch ((ItemNames)item.Value.Gem2)
                {
                    case ItemNames.ShortNormalPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.05f);
                        break;
                    case ItemNames.ShortRefPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.1f);
                        break;
                    case ItemNames.ShortSuperPhoenixGem:
                        magicAttack = (int)(magicAttack * 0.15f);
                        break;
                    case ItemNames.ShortNormalDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.05f);
                        break;
                    case ItemNames.ShortRefDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.1f);
                        break;
                    case ItemNames.ShortSuperDragonGem:
                        minimumPhsyicalAttack += (int)(minimumPhsyicalAttack * 0.15f);
                        break;
                    case ItemNames.ShortNormalFuryGem:
                        dodge += (int)(dodge * 0.05f);
                        break;
                    case ItemNames.ShortRefFuryGem:
                        dodge += (int)(dodge * 0.1f);
                        break;
                    case ItemNames.ShortSuperFuryGem:
                        dodge += (int)(dodge * 0.15f);
                        break;
                    case ItemNames.ShortNormalRainbowGem:
                        entity.ExpBonus += 0.10f;
                        break;
                    case ItemNames.ShortRefRainbowGem:
                        entity.ExpBonus += 0.15f;
                        break;
                    case ItemNames.ShortSuperRainbowGem:
                        entity.ExpBonus += 0.25f;
                        break;
                    case ItemNames.ShortNormalVioletGem:
                        entity.WeaponExpBonus += 0.30f;
                        break;
                    case ItemNames.ShortRefVioletGem:
                        entity.WeaponExpBonus += 0.50f;
                        break;
                    case ItemNames.ShortSuperVioletGem:
                        entity.WeaponExpBonus += 1.00f;
                        break;
                    case ItemNames.ShortNormalMoonGem:
                        entity.MagicExpBonus += 0.30f;
                        break;
                    case ItemNames.ShortRefMoonGem:
                        entity.MagicExpBonus += 0.50f;
                        break;
                    case ItemNames.ShortSuperMoonGem:
                        entity.MagicExpBonus += 1.00f;
                        break;
                    case ItemNames.ShortNormalTortoiseGem:
                        defense -= (int)(defense * 0.02f);
                        break;
                    case ItemNames.ShortRefTortoiseGem:
                        defense -= (int)(defense * 0.04f);
                        break;
                    case ItemNames.ShortSuperTortoiseGem:
                        defense -= (int)(defense * 0.06f);
                        break;
                }
                if (!Collections.ItemBonus.ContainsKey(item.Value.BonusId))
                    continue;
                maximumHp += Collections.ItemBonus[item.Value.BonusId].life;
                defense += Collections.ItemBonus[item.Value.BonusId].defense;
                dodge += (ushort)Collections.ItemBonus[item.Value.BonusId].dexterity;
                dodge += (ushort)Collections.ItemBonus[item.Value.BonusId].dodge;
                magicAttack += Collections.ItemBonus[item.Value.BonusId].magic_atk;
                magicDefense += Collections.ItemBonus[item.Value.BonusId].magic_def;
                maximumPhsyicalAttack += Collections.ItemBonus[item.Value.BonusId].attack_max;
                minimumPhsyicalAttack += Collections.ItemBonus[item.Value.BonusId].attack_min;
            }

            entity.MinimumPhsyicalAttack = minimumPhsyicalAttack;
            entity.MaximumPhsyicalAttack = maximumPhsyicalAttack;
            entity.Defense = defense;
            entity.Dexterity = entity.Dexterity;
            entity.MagicDefense = magicDefense;
            entity.MagicAttack = magicAttack;
            entity.MaximumHp = (ushort)maximumHp;
            entity.MaximumMp = (ushort)maximumMp;

            var arrows = entity.GetEquip(MsgItemPosition.LeftWeapon);
            if (arrows.Valid())
            {
                if (arrows.ItemId / 10 == 105010)
                {
                    entity.MinimumPhsyicalAttack = (int)(entity.MinimumPhsyicalAttack * 0.85);
                    entity.MaximumPhsyicalAttack = (int)(entity.MaximumPhsyicalAttack * 0.85);
                }

                if (arrows.ItemId / 10 == 105020)
                {
                    entity.AttackSpeed = 500;
                    entity.MinimumPhsyicalAttack = 0;
                    entity.MaximumPhsyicalAttack = 0;
                }

                if (arrows.ItemId / 10 == 105040)
                {
                    entity.MinimumPhsyicalAttack = (int)(entity.MinimumPhsyicalAttack * 1.15);
                    entity.MaximumPhsyicalAttack = (int)(entity.MaximumPhsyicalAttack * 1.15);
                }
            }

            if (entity.HasFlag(StatusEffect.MagicShield) || entity.HasFlag(StatusEffect.XpShield))
                entity.Defense = entity.Defense;
            if (entity.HasFlag(StatusEffect.StarOfAccuracy) || entity.HasFlag(StatusEffect.XpAccuracy))
                entity.AttackSpeed = (int)(entity.AttackSpeed * Math.Max(1, entity.AttackSpeedBonus));

            entity.AttackSpeed -= 250;
            if (entity.AttackSpeed < 500)
                entity.AttackSpeed = 500;

            if (entity.HasFlag(StatusEffect.Cyclone))
                entity.AttackSpeed /= 2;
            dodge += entity.Agility + 25;
            entity.Dexterity += (ushort)dodge;
        }
    }
}