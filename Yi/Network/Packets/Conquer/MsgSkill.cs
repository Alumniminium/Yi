using System.Runtime.InteropServices;
using Yi.Network.Sockets;
using Yi.Structures;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgSkill
    {
        public ushort Size;
        public ushort Id;
        public uint Experience;
        public ushort SkillId;
        public ushort Level;

        public static byte[] Create(Skill skill)
        {
            var packet = new MsgSkill
            {
                Size = (ushort)sizeof(MsgSkill),
                Id = 1103, SkillId = skill.Id, Experience = skill.Experience, Level = skill.Level};
            return packet;
        }

        public static unsafe implicit operator byte[](MsgSkill msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgSkill*)p = *&msg;
            return buffer;
        }
    }
}