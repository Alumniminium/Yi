using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PassageInfo
    {
        public int PosX;
        public int PosY;
        public int Index;
    };
}