using System;
using System.IO;
using System.Text;

namespace Yi.Network.Sockets
{
    public class Packet : IDisposable
    {
        private readonly MemoryStream _ms;

        public Packet(int type, int size)
        {
            _ms = new MemoryStream(BufferPool.GetBuffer());

            Short((short)size);
            Short((short)type);
        }

        public void Dispose() => _ms.Dispose();

        public Packet Goto(int i)
        {
            _ms.Seek(i, SeekOrigin.Begin);
            return this;
        }

        public Packet Int(uint value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 4);
            return this;
        }

        public Packet Int(int value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 4);
            return this;
        }

        public Packet Int(uint? value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);
            if (value.HasValue)
                _ms.Write(BitConverter.GetBytes(value.Value), 0, 4);
            else
                return Skip(4);
            return this;
        }

        public Packet Long(ulong value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 8);
            return this;
        }

        public Packet Short(short value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 2);
            return this;
        }

        public Packet Short(ushort value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 2);
            return this;
        }

        public Packet Byte(byte value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 1);
            return this;
        }

        public Packet Byte(sbyte value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 1);
            return this;
        }

        public Packet Byte(bool value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);

            _ms.Write(BitConverter.GetBytes(value), 0, 1);
            return this;
        }

        public Packet Skip(int count)
        {
            _ms.Seek(count, SeekOrigin.Current);
            return this;
        }

        public Packet String(string value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);
            if (value == null)
                return this;
            Byte((byte)Math.Min(255, value.Length));
            _ms.Write(Encoding.ASCII.GetBytes(value), 0, Math.Min(255, value.Length));
            return this;
        }

        public Packet StringWithoutLenght(string value, int offset = -1)
        {
            if (offset != -1)
                _ms.Seek(offset, SeekOrigin.Begin);
            _ms.Write(Encoding.ASCII.GetBytes(value), 0, value.Length);
            return this;
        }

        public static implicit operator byte[](Packet packet) => packet._ms.ToArray();

        public void FinishPacket()
        {
            var len = (ushort)_ms.Position;
            _ms.Seek(0, SeekOrigin.Begin);
            _ms.Write(BitConverter.GetBytes(len),0,2);
        }
    }
}