using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MySize
    {
        public int Width;
        public int Height;

        public MySize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    };
}