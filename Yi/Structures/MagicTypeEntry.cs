using System;
using Yi.Enums;

namespace Yi.Structures
{
    [Serializable]
    public struct MagicTypeEntry
    {
        public byte Level;
        public MagicTypeEnum Type;
        public bool Crime;
        public bool Ground;
        public uint Multi;
        public uint Target;
        public uint MpCost;
        public int Power;
        public uint IntoneDuration;
        public byte Success;
        public uint Time;
        public uint Distance;
        public uint Status;
        public uint ExpRequired;
        public byte LevelRequired;
        public byte Xp;
        public ushort WeaponSubType;
        public uint ActiveTime;
        public bool AutoActive;
        public uint FloorAttribute;
        public bool AutoLearn;
        public byte LearnLevel;
        public byte StaminaCost;
        public bool WeaponHit;
        public uint Delay;
        public uint TargetDelay;
        public bool CanBeusedInMarket;
        public uint TargetWoundDelay;
        public uint Range;
    }
}