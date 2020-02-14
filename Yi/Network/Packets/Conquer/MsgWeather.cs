using System.Runtime.InteropServices;
using Yi.Enums;
using Yi.Network.Sockets;

namespace Yi.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MsgWeather
    {
        public ushort Size;
        public ushort Id;
        public WeatherType Type;
        public int Intensity;
        public int Direction;
        public int Color;

        public static unsafe byte[] Create(WeatherType weather, int intensity, int direction, int color)
        {
            var packet = new MsgWeather
            {
                Size = (ushort) sizeof(MsgWeather),
                Id = 1016,
                Type = weather,
                Intensity=intensity,
                Color = color,
                Direction = direction
            };
            return packet;
        }
        
        public static unsafe implicit operator byte[] (MsgWeather msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgWeather*)p = *&msg;
            return buffer;
        }
    }
}