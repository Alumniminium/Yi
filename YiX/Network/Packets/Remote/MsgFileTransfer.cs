using System.Runtime.InteropServices;
using YiX.Network.Sockets;

namespace YiX.Network.Packets.Remote
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgFileTransfer
    {
        public ushort Size { get; set; }
        public ushort Id { get; set; }
        public int TotalLen;
        public int ChunkSize;
        public unsafe fixed byte Chunk[1490];

        public static unsafe byte[] Create(byte[] chunk, int totalLen)
        {
            var packet = new MsgFileTransfer
            {
                Size = (ushort) (chunk.Length + 8),
                Id = 5,
                TotalLen = totalLen,
                ChunkSize = chunk.Length
            };
            for (var i = 0; i < 1490; i++)
                packet.Chunk[i] = chunk[i];
            return packet;
        }

        public static unsafe implicit operator byte[](MsgFileTransfer msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgFileTransfer*)p = *&msg;
            return buffer;
        }

        public static unsafe implicit operator MsgFileTransfer(byte[] buffer)
        {
            MsgFileTransfer packet;
            fixed (byte* p = buffer)
                packet = *(MsgFileTransfer*)p;
            BufferPool.RecycleBuffer(buffer);
            return packet;
        }
    }
}