// -------- Yi --------
// Project: Library File: AttackResult.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

namespace YiX.Enums
{
    public enum AttackResult : sbyte
    {
        Success,
        AttackerDead,
        TargetDead,
        WrongMap,
        TooFar,
        PkMode,
        MapFlags,
        TargetFlying,
        Fail = 127
    }
}