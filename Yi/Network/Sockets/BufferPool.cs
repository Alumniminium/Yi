using System;
using System.Collections.Concurrent;

namespace Yi.Network.Sockets
{
    [Serializable]
    public static class BufferPool
    {
        public static int Count => Pool.Count;

        public const int MIN_BIG_POOL_SIZE = 500;
        public const int MAX_BUFFER_SIZE_BYTES = 850;

        private static readonly ConcurrentQueue<byte[]> Pool = new ConcurrentQueue<byte[]>();

        public static byte[] GetBuffer()
        {
            byte[] buffer;

            while (!Pool.TryDequeue(out buffer))
                FillPool();

            return buffer;
        }

        public static byte[] Clone(byte[] packet)
        {
            var copy = GetBuffer();
            try
            {
                Buffer.BlockCopy(packet, 0, copy, 0, packet.Length);
                return copy;
            }
            catch
            {
                Output.WriteLine($"packet len: {packet.Length} copylen: {copy.Length}");
                throw;
            }
        }

        public static void RecycleBuffer(byte[] buffer)
        {
            if (buffer.Length < MAX_BUFFER_SIZE_BYTES)
                return;

            Array.Clear(buffer, 0, buffer.Length);
            Pool.Enqueue(buffer);
        }

        private static void FillPool()
        {
            if (Pool.Count > MIN_BIG_POOL_SIZE)
                return;
            for (var i = 0; i < MIN_BIG_POOL_SIZE; i++)
                Pool.Enqueue(new byte[MAX_BUFFER_SIZE_BYTES]);
            Output.WriteLine($"Yi Socket's Big BufferPool Expanded to {Count}");
        }
    }
}