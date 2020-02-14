namespace Yi.Network.Packets
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public unsafe partial struct MsgMagicEffectEx : Packet
    //{
    //public ushort Size { get; }
    //public ushort Id { get; }
    //public int UniqId;
    //public int Param; //TargetUID || (X, Y)
    //public short Type;
    //public short Level;
    //public int TargetCount;
    ////public SkillTarget[] Targets;

    //public MsgMagicEffectEx(int uniqId, int param, short type, short level, SkillTarget[] targets)
    //{
    //    Size = (ushort)(20 + (targets.Length * 12));
    //    Id = 1105;
    //    UniqId = uniqId;
    //    Param = param;
    //    Type = type;
    //    Level = level;
    //    TargetCount = targets.Length;
    //    Targets = targets;
    //}
    //public static implicit operator byte[] (MsgSynMemberInfo msg)
    //{
    //    var packet = BufferPool.GetSmallBuffer();
    //    fixed (byte* ptr = packet)
    //        Marshal.StructureToPtr(msg, (IntPtr)ptr, true);
    //    return packet;
    //}
    //public static implicit operator MsgSynMemberInfo(byte[] msg)
    //{
    //    MsgSynMemberInfo packet;
    //    fixed (byte* ptr = msg)
    //        packet = (MsgSynMemberInfo)Marshal.PtrToStructure((IntPtr)ptr, typeof(MsgSynMemberInfo));
    //    return packet;
    //}
    //}
}