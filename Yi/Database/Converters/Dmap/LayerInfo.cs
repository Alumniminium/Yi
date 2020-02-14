using System.Runtime.InteropServices;

namespace Yi.Database.Converters.Dmap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LayerInfo
    {
        public ushort Terrain;
        public ushort Mask; //GroundLayer
        public short Altitude;
    };
}