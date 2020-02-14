using System.Collections.Generic;
using Yi.AttackSystems.AttackCalculations;
using Yi.Enums;

namespace Yi.Structures
{
    public static class MagicTypeHelper
    {
        private static readonly Dictionary<MagicTypeEnum, TargetingType> TargetTypeLookup = new Dictionary<MagicTypeEnum, TargetingType>
        {
            [MagicTypeEnum.DamageSingle] = TargetingType.Single,
            [MagicTypeEnum.HealSingle] = TargetingType.Single,
            [MagicTypeEnum.AttackCrossHp] = TargetingType.None,
            [MagicTypeEnum.AttackSectorHp] = TargetingType.Sector,
            [MagicTypeEnum.AttackRoundHp] = TargetingType.Circle,
            [MagicTypeEnum.AttackSingleStatus] = TargetingType.Single,
            [MagicTypeEnum.RecoverSingleStatus] = TargetingType.Single,
            [MagicTypeEnum.Square] = TargetingType.None,
            [MagicTypeEnum.Jumpattack] = TargetingType.None,
            [MagicTypeEnum.Randomtrans] = TargetingType.None,
            [MagicTypeEnum.Dispatchxp] = TargetingType.None,
            [MagicTypeEnum.Collide] = TargetingType.None,
            [MagicTypeEnum.Serialcut] = TargetingType.None,
            [MagicTypeEnum.Line] = TargetingType.None,
            [MagicTypeEnum.Atkrange] = TargetingType.None,
            [MagicTypeEnum.Atkstatus] = TargetingType.None,
            [MagicTypeEnum.CallTeammember] = TargetingType.None,
            [MagicTypeEnum.Recordtransspell] = TargetingType.None,
            [MagicTypeEnum.Transform] = TargetingType.None,
            [MagicTypeEnum.Addmana] = TargetingType.None,
            [MagicTypeEnum.Laytrap] = TargetingType.None,
            [MagicTypeEnum.Dance] = TargetingType.None,
            [MagicTypeEnum.Callpet] = TargetingType.None,
            [MagicTypeEnum.Vampire] = TargetingType.None,
            [MagicTypeEnum.Instead] = TargetingType.None,
            [MagicTypeEnum.Declife] = TargetingType.None,
            [MagicTypeEnum.Groundsting] = TargetingType.None,
            [MagicTypeEnum.Reborn] = TargetingType.None,
            [MagicTypeEnum.TeamMagic] = TargetingType.None,
            [MagicTypeEnum.BombLockall] = TargetingType.None,
            [MagicTypeEnum.SorbSoul] = TargetingType.None,
            [MagicTypeEnum.Steal] = TargetingType.None,
            [MagicTypeEnum.LinePenetrable] = TargetingType.None,
            [MagicTypeEnum.BlastThunder] = TargetingType.None,
            [MagicTypeEnum.MultiAttachstatus] = TargetingType.None,
            [MagicTypeEnum.MultiDetachstatus] = TargetingType.None,
            [MagicTypeEnum.MultiCure] = TargetingType.None,
            [MagicTypeEnum.StealMoney] = TargetingType.None,
            [MagicTypeEnum.Ko] = TargetingType.None,
            [MagicTypeEnum.Escape] = TargetingType.None,
            [MagicTypeEnum.FlashAttack] = TargetingType.None,
            [MagicTypeEnum.AttrackMonster] = TargetingType.None,
        };

        private static readonly Dictionary<MagicTypeEnum, UseType> UseTypeLookup = new Dictionary<MagicTypeEnum, UseType>
        {
            [MagicTypeEnum.DamageSingle] = UseType.Damage,
            [MagicTypeEnum.HealSingle] = UseType.Heal,
            [MagicTypeEnum.AttackCrossHp] = UseType.None,
            [MagicTypeEnum.AttackSectorHp] = UseType.Damage,
            [MagicTypeEnum.AttackRoundHp] = UseType.Damage,
            [MagicTypeEnum.AttackSingleStatus] = UseType.Buff,
            [MagicTypeEnum.RecoverSingleStatus] = UseType.Buff,
            [MagicTypeEnum.Square] = UseType.None,
            [MagicTypeEnum.Jumpattack] = UseType.None,
            [MagicTypeEnum.Randomtrans] = UseType.None,
            [MagicTypeEnum.Dispatchxp] = UseType.None,
            [MagicTypeEnum.Collide] = UseType.None,
            [MagicTypeEnum.Serialcut] = UseType.None,
            [MagicTypeEnum.Line] = UseType.None,
            [MagicTypeEnum.Atkrange] = UseType.None,
            [MagicTypeEnum.Atkstatus] = UseType.None,
            [MagicTypeEnum.CallTeammember] = UseType.None,
            [MagicTypeEnum.Recordtransspell] = UseType.None,
            [MagicTypeEnum.Transform] = UseType.None,
            [MagicTypeEnum.Addmana] = UseType.None,
            [MagicTypeEnum.Laytrap] = UseType.None,
            [MagicTypeEnum.Dance] = UseType.None,
            [MagicTypeEnum.Callpet] = UseType.None,
            [MagicTypeEnum.Vampire] = UseType.None,
            [MagicTypeEnum.Instead] = UseType.None,
            [MagicTypeEnum.Declife] = UseType.None,
            [MagicTypeEnum.Groundsting] = UseType.None,
            [MagicTypeEnum.Reborn] = UseType.None,
            [MagicTypeEnum.TeamMagic] = UseType.None,
            [MagicTypeEnum.BombLockall] = UseType.None,
            [MagicTypeEnum.SorbSoul] = UseType.None,
            [MagicTypeEnum.Steal] = UseType.None,
            [MagicTypeEnum.LinePenetrable] = UseType.None,
            [MagicTypeEnum.BlastThunder] = UseType.None,
            [MagicTypeEnum.MultiAttachstatus] = UseType.None,
            [MagicTypeEnum.MultiDetachstatus] = UseType.None,
            [MagicTypeEnum.MultiCure] = UseType.None,
            [MagicTypeEnum.StealMoney] = UseType.None,
            [MagicTypeEnum.Ko] = UseType.None,
            [MagicTypeEnum.Escape] = UseType.None,
            [MagicTypeEnum.FlashAttack] = UseType.None,
            [MagicTypeEnum.AttrackMonster] = UseType.None,
        };

        public static UseType Convert(MagicTypeEnum type)
        {
            UseTypeLookup.TryGetValue(type, out var found);
            return found;
        }
        public static TargetingType ConvertTargetingType(MagicTypeEnum type)
        {
            TargetTypeLookup.TryGetValue(type, out var found);
            return found;
        }
    }
}