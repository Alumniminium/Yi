using System.Collections.Generic;

namespace Yi.Calculations
{
    public static class Constants
    {
        public static readonly HashSet<ushort> LoopableSkills = new HashSet<ushort> {1000, 1001, 1002, 1180, 1165, 1160, 1150};
        public static readonly sbyte[] DeltaX = {0, -1, -1, -1, 0, 1, 1, 1};
        public static readonly sbyte[] DeltaY = {1, 1, 0, -1, -1, -1, 0, 1};
        public const string AnswerOk = "ANSWER_OK";
        public const string Allusers = "ALLUSERS";
        public const string System = "SYSTEM";
    }
}