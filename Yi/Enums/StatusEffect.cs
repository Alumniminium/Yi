using System;

namespace Yi.Enums
{
    [Flags]
    public enum StatusEffect : ulong
    {
        None = 0x0,
        Flashing = 0x1,
        Poisoned = 0x2,
        Unknow = 0x8,
        XpList = 0x10,
        Frozen = 0x20,
        TeamLeader = 0x40,
        StarOfAccuracy = 0x80,
        MagicShield = 0x100,
        Stigma = 0x200,
        Die = 0x400,
        Fade = 0x800,
        XpAccuracy = 0x1000,
        XpShield = 0x2000,
        RedName = 0x4000,
        BlackName = 0x8000,
        SpawnProtection = 0x10000,
        SuperMan = 0x40000,
        Invisibility = 0x400000,
        Cyclone = 0x800000,
        Flying = 0x8000000,
        CastingPray = 0x40000000,
        Praying = 0x80000000,
    }
}