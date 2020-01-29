using System;
// ReSharper disable All

namespace YiX.Enums
{
    [Flags]
    public enum AIType
    {
        Base =          0x00000000000,
        BasicMob=       0x00000000001,
        MagicMob=       0x00000000010,
        BossMob=        0x00000000100,
        Friendly =      0x00000001000,
        Aggressive =    0x00000010000,
        BasicBot =      0x00000100000,
        MercenaryBot =  0x00001000000,
        Pet = BasicMob|MagicMob|Friendly,
    }
}
