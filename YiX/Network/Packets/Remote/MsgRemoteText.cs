using System;
using System.Linq;
using System.Runtime.InteropServices;
using YiX.Network.Sockets;
using YiX.World;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Remote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgRemoteText
    {
        public ushort Size { get; set; }
        public ushort Id { get; set; }
        public fixed byte Header[16];
        public fixed byte Message[512];

        public static byte[] Create(string header, string message)
        {
            var packet = new MsgRemoteText
            {
                Size = 532,
                Id = 2
            };
            for (var i = 0; i < header.Length; i++)
                packet.Header[i] = (byte)header[i];

            for (var i = 0; i < message.Length; i++)
                packet.Message[i] = (byte)message[i];
            return packet;
        }

        internal static byte[] Create(ConsoleColor colorCode, string message)
        {
            var header = "Info";
            switch (colorCode)
            {
                case ConsoleColor.White:
                case ConsoleColor.Cyan:
                case ConsoleColor.Blue:
                    break;
                case ConsoleColor.Green:
                    header = "Win";
                    break;
                case ConsoleColor.Red:
                    header = "ERROR";
                    break;
            }
            var packet = new MsgRemoteText
            {
                Size = 532,
                Id = 2
            };

            for (var i = 0; i < header.Length; i++)
                packet.Header[i] = (byte)header[i];

            for (var i = 0; i < message.Length; i++)
                packet.Message[i] = (byte)message[i];
            return packet;
        }

        public static void Handle(Player account, byte[] buffer)
        {
            MsgRemoteText packet = buffer;
            foreach (var mapObject in GameWorld.Maps.Values.SelectMany(value => value.Entities.Values).OfType<Player>())
            {
                //mapObject.Send(MsgText.CreateFor(packet.Header, Constants.ALLUSERS, packet.Message, MsgTextType.Service));
            }
        }
        public static implicit operator byte[] (MsgRemoteText msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgRemoteText*)p = *&msg;
            return buffer;
        }

        public static implicit operator MsgRemoteText(byte[] buffer)
        {
            MsgRemoteText packet;
            fixed (byte* p = buffer)
                packet = *(MsgRemoteText*)p;
            BufferPool.RecycleBuffer(buffer);
            return packet;
        }
    }
}