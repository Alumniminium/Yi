using System;
using System.Security.Cryptography;

namespace YiX.Helpers
{
    public static class SafeRandom
    {
        private const int PoolSize = 2048;

        private static readonly Lazy<RandomNumberGenerator> Rng = new Lazy<RandomNumberGenerator>(() => new RNGCryptoServiceProvider());

        private static readonly Lazy<object> PositionLock = new Lazy<object>(() => new object());

        private static readonly Lazy<byte[]> Pool = new Lazy<byte[]>(() => GeneratePool(new byte[PoolSize]));

        private static int _bufferPosition;

        public static int GetNext()
        {
            while (true)
            {
                var result = (int)(GetRandomUInt32() & int.MaxValue);

                if (result != int.MaxValue)
                    return result;
            }
        }

        public static int GetNext(int maxValue)
        {
            if (maxValue < 1)
                throw new ArgumentException("Must be greater than zero.", nameof(maxValue));
            return GetNext(0, maxValue);
        }

        public static int GetNext(int minValue, int maxValue)
        {
            const long max = 1 + (long)uint.MaxValue;

            if (minValue >= maxValue)
                minValue--;

            long diff = maxValue - minValue;
            var limit = max - max % diff;

            while (true)
            {
                var rand = GetRandomUInt32();
                if (rand < limit)
                    return (int)(minValue + rand % diff);
            }
        }

        public static int Next(int maxValue) => GetNext(0, maxValue);

        public static int Next(int minValue, int maxValue) => GetNext(minValue, maxValue);

        private static byte[] GeneratePool(byte[] buffer)
        {
            _bufferPosition = 0;
            Rng.Value.GetBytes(buffer);
            return buffer;
        }

        private static uint GetRandomUInt32()
        {
            uint result;
            lock (PositionLock.Value)
            {
                if (PoolSize - _bufferPosition < sizeof(uint))
                    GeneratePool(Pool.Value);

                result = BitConverter.ToUInt32(Pool.Value, _bufferPosition);
                _bufferPosition += sizeof(uint);
            }

            return result;
        }
    }
}