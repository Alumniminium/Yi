using System;
using System.Text;

namespace YiX.Helpers
{
    public static class StringExtensions
    {
        public static readonly StringBuilder Builder = new StringBuilder();
        public static readonly object Size16Lock = new object();
        public static readonly object Size256Lock = new object();

        public static uint ToHex(this string hex)
        {
            if (hex == null)
                return 0;

            try
            {
                var result = Convert.ToUInt32(hex, 16);

                return result;
            }
            catch
            {
                return 0;
            }
        }
        public static string Size16(this string str)
        {
            if (str.Length > 15)
                return str.Substring(0, 15);
            lock (Size16Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                while (Builder.Length < 15)
                {
                    Builder.Append('\0');
                }
                return Builder.ToString();
            }
        }
        public static string Size16(this string str, char fill)
        {
            if (str.Length > 15)
                return str.Substring(0, 15);
            lock (Size16Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                while (Builder.Length < 15)
                {
                    if (Builder.Length % 2 == 0)
                        Builder.Insert(0, fill);
                    else
                        Builder.Append(fill);
                }
                return Builder.ToString();
            }
        }
        public static string Size32(this string str, char fill)
        {
            if (str.Length > 32)
                return str.Substring(0, 32);
            lock (Size16Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                while (Builder.Length < 32)
                {
                    if (Builder.Length%2 == 0)
                        Builder.Insert(0, fill);
                    else
                        Builder.Append(fill);
                }
                return Builder.ToString();
            }
        }

        public static string Size16Offline(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            lock (Size16Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                for (var i = str.Length; i < 15; i++)
                    Builder.Append('\0');
                Builder.Insert(0, "[OFF]");
                Builder.Remove(15, Builder.Length - 15);
                return Builder.ToString();
            }
        }
        public static string Size16AFK(this string str)
        {
            lock (Size16Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                for (var i = str.Length; i < 15; i++)
                    Builder.Append('\0');
                Builder.Insert(0, "[AFK]");
                Builder.Remove(15, Builder.Length - 15);
                return Builder.ToString();
            }
        }

        public static string Size256(this string str)
        {
            if (str.Length > 255)
                return str.Substring(0, 255);
            lock (Size256Lock)
            {
                Builder.Clear();
                Builder.Append(str);
                for (var i = str.Length; i < 255; i++)
                    Builder.Append('\0');
                return Builder.ToString();
            }
        }
    }
}