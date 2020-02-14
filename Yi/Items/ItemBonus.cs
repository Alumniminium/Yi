using System.Runtime.InteropServices;

namespace Yi.Items
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ItemBonus
    {
        public short Life;
        public short MaxAtk;
        public short MinAtk;
        public short Defence;
        public short MAtk;
        public short MDef;
        public short Dexterity;
        public short Dodge;
    }
}