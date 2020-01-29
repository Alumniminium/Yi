using System.Globalization;
using System.Linq;

namespace YiX.Helpers
{
    public static class ByteExtensions
    {
        private static string StrHexToAnsi(string strHex)
        {
            var data = strHex.Split(' ');
            return (from tmpHex in data where tmpHex != "" select byte.Parse(tmpHex, NumberStyles.HexNumber)).Aggregate("", (current, byteData) => current + (byteData >= 32 && byteData <= 126 ? ((char)byteData).ToString() : "."));
        }
        public static string HexDump(this byte[] bytes)
        {
            var hex = bytes.Aggregate("", (current, b) => current + b.ToString("X2") + " ");
            var Out = "";
            while (hex.Length != 0)
            {
                var subString = hex.Substring(0, hex.Length >= 48 ? 48 : hex.Length);
                var remove = subString.Length;
                subString = subString.PadRight(60, ' ') + StrHexToAnsi(subString);
                hex = hex.Remove(0, remove);
                Out += subString + "\r\n";
            }
            return Out;
        }
    }
}