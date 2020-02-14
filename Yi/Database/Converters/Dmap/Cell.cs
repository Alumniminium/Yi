using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cell
    {
        public readonly bool Accessible;
        public readonly short Altitude;

        public Cell(LayerInfo info)
        {
            Accessible = info.Mask == 0;
            Altitude = info.Altitude;
        }
    }
}