using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MyPos
    {
        public int X;
        public int Y;

        public MyPos(int x, int y)
        {
            X = x;
            Y = y;
        }
    };
}