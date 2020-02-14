using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RegionInfo
    {
        public uint IdMap;
        public MyPos PosCell;
        public uint Type;
        public uint Cx;
        public uint Cy;
        public fixed byte String1 [128];
        public fixed byte String2 [128];
        public fixed byte String3 [128];
        public int NColor;
        public int NShowType;
        public int DColor;
        public int DShowPos;
        public int DShowType;
        public int DAccess;
        public uint AccessTime;
        public int Access; //Boolean
    };
}