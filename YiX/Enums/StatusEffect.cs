using System;

namespace YiX.Enums
{
    [Flags]
    public enum StatusEffect : long
    {
        None =              0b0,
        Flashing =          0b1,
        Poisoned =          0b10,
        Unknow =            0b1000,
        XpList =            0b10000,
        Frozen =            0b100000,
        TeamLeader =        0b1000000,
        StarOfAccuracy =    0b10000000,
        MagicShield =       0b100000000,
        Stigma =            0b1000000000,
        Die =               0b10000000000,
        Fade =              0b100000000000,
        XpAccuracy =        0b1000000000000,
        XpShield =          0b10000000000000,
        RedName =           0b100000000000000,
        BlackName =         0b1000000000000000,
        SpawnProtection =   0b10000000000000000,
        SuperMan =          0b1000000000000000000,
        Invisibility =      0b10000000000000000000000,
        Cyclone =           0b100000000000000000000000,
        Flying =            0b1000000000000000000000000000,
        CastingPray =       0b1000000000000000000000000000000,
        Praying =           0b10000000000000000000000000000000,
    }
}