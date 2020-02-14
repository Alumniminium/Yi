using System;
using System.Drawing;
using System.Runtime.InteropServices;
using YiX.Entities;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgColor
    {
        public ushort Size;
        public ushort Id;
        public int Timestamp;
        public int UniqueId;
        public uint Rgb;
        public ushort X;
        public ushort Y;
        public ushort Direction;
        public uint Type;

        public static byte[] Create(YiObj target, Color color)
        {
            var packet = new MsgColor
            {
                Size = (ushort)sizeof(MsgColor),
                Id = 1010,
                Timestamp = Environment.TickCount,
                UniqueId = target.UniqueId,
                X = target.Location.X,
                Y = target.Location.Y,
                Direction = (ushort)target.Direction,
                Rgb = ColorToUInt(color),
                Type = 104
            };
            return packet;
        }
        public static byte[] Create(YiObj target, uint color)
        {
            var packet = new MsgColor
            {
                Size = (ushort)sizeof(MsgColor),
                Id = 1010,
                Timestamp = Environment.TickCount,
                UniqueId = target.UniqueId,
                X = target.Location.X,
                Y = target.Location.Y,
                Direction = (ushort)target.Direction,
                Rgb = color,
                Type = 104
            };
            return packet;
        }

        private static uint ColorToUInt(Color color) => (uint)((color.A << 24) | (color.R << 16) |
                                                        (color.G << 8) | (color.B << 0));

        public static unsafe implicit operator byte[] (MsgColor msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgColor*)p = *&msg;
            return buffer;
        }
    }
}