using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YiX.Helpers
{
    public static class MsgSerializer
    {
        public static byte[] ToBuffer(this object msg)
        {
            var size = Marshal.SizeOf(msg);
            var buffer = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(msg, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            return buffer;
        }
        public static T ToMsg<T>(this byte[] buffer)
        {
            var msg = default(T);
            if (msg == null)
            {
                Debugger.Break();
                return default(T);
            }

            var size = Marshal.SizeOf(msg);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, 0, ptr, size);

            msg = (T)Marshal.PtrToStructure(ptr, msg.GetType());
            Marshal.FreeHGlobal(ptr);

            return msg;
        }
    }
}