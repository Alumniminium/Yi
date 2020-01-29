using System.Collections;

namespace YiX.Helpers
{
    public static class BitArrayExtensions
    {
        public static byte[] BitArrayToByteArray(this BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}