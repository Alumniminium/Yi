using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Yi.Database.Converters
{
    public static unsafe class NativeMethods
    {
        [DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetSystemTimes(out FILETIME lpIdleTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

        public static void MinimizeFootprint() => EmptyWorkingSet(Process.GetCurrentProcess().Handle);

        public static void Memcpy(void* dest, byte[] src, int size) => Marshal.Copy(src, 0, (IntPtr)dest, size);

        public static void Memcpy(byte[] dest, void* src, int size) => Marshal.Copy((IntPtr)src, dest, 0, size);

        public static void Memcpy(void* dest, void* src, int size)
        {
            var count = size / sizeof(int);
            for (var i = 0; i < count; i++)
                *((int*)dest + i) = *((int*)src + i);

            var pos = size - size % sizeof(int);
            for (var i = 0; i < size % sizeof(int); i++)
                *((byte*)dest + pos + i) = *((byte*)src + pos + i);
        }

        public static void Free(void* ptr)
        {
            if (ptr != null)
                Marshal.FreeHGlobal((IntPtr)ptr);
        }

        public static void* Malloc(int size)
        {
            var ptr = Marshal.AllocHGlobal(size).ToPointer();
            return ptr;
        }

        public static void* Calloc(int size)
        {
            var ptr = Marshal.AllocHGlobal(size).ToPointer();

            var count = size / sizeof(int);
            for (var i = 0; i < count; i++)
                *((int*)ptr + i) = 0;

            var pos = size - size % sizeof(int);
            for (var i = 0; i < size % sizeof(int); i++)
                *((byte*)ptr + pos + i) = 0;

            return ptr;
        }

        public static string Cstring(byte* src, int size)
        {
            if (src == null)
                return null;

            var builder = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                if (src[i] == '\0')
                    break;

                builder.Append((char)src[i]);
            }
            return builder.ToString();
        }
    }
}