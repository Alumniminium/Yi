using System.IO;
using Yi.Database.Converters;

namespace Yi.Helpers
{
    public static unsafe class StreamExtensions
    {
        public static int Read(this Stream stream, void* pBuf, int size)
        {
            var buffer = new byte[size];
            var read = stream.Read(buffer, 0, size);
            NativeMethods.Memcpy(pBuf, buffer, read);
            return read;
        }
    }
}