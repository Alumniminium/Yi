// * ************************************************************
// * * START:                                          corc5.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Conquer Online Rivest Cipher 5 for the library.
// * corc5.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (May 10th, 2011)
// * Copyright (C) 2011 CptSky
// * 
// * ************************************************************

using System;

namespace CO2_CORE_DLL.Security.Cryptography
{
    /// <summary>
    /// Conquer Online Rivest Cipher 5
    /// </summary>
    public unsafe class CORC5
    {
        private const Int32 RC5_32 = 32;
        private const Int32 RC5_12 = 12;
        private const Int32 RC5_SUB = (RC5_12 * 2 + 2);
        private const Int32 RC5_16 = 16;
        private const Int32 RC5_KEY = (RC5_16 / 4);

        private UInt32* BufKey = null;
        private UInt32* BufSub = null;
        private UInt32 RC5_PW32 = 0xB7E15163;
        private UInt32 RC5_QW32 = 0x9E3779B9;

        /// <summary>
        /// Create a new RC5 instance.
        /// </summary>
        public CORC5() { }

        /// <summary>
        /// Create a new RC5 instance with the specified magics numbers. (Shouldn't be used)
        /// </summary>
        public CORC5(UInt32 RC5_PW32, UInt32 RC5_QW32)
        {
            this.RC5_PW32 = RC5_PW32;
            this.RC5_QW32 = RC5_QW32;
        }

        ~CORC5()
        {
            if (BufKey != null)
                Kernel.free(BufKey);
            if (BufSub != null)
                Kernel.free(BufSub);
        }

        /// <summary>
        /// Generates a random key (Key) to use for the algorithm.
        /// CO2: { 0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E, 0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE }
        /// </summary>
        public void GenerateKey(Byte* pBufKey, Int32 Length)
        {
            Kernel.assert(pBufKey != null);
            Kernel.assert(Length > 0 && Length == RC5_16);

            if (BufKey != null)
                Kernel.free(BufKey);
            if (BufSub != null)
                Kernel.free(BufSub);

            BufKey = (UInt32*)Kernel.malloc(RC5_KEY * sizeof(UInt32));
            BufSub = (UInt32*)Kernel.malloc(RC5_SUB * sizeof(UInt32));

            for (var z = 0; z < RC5_KEY; z++)
                BufKey[z] = ((UInt32*)pBufKey)[z];

            BufSub[0] = RC5_PW32;
            Int32 i, j, k;
            for (i = 1; i < RC5_SUB; i++)
                BufSub[i] = BufSub[i - 1] - RC5_QW32;

            UInt32 x, y;
            i = j = 0;
            x = y = 0;
            for (k = 0; k < 3 * Math.Max(RC5_KEY, RC5_SUB); k++)
            {
                BufSub[i] = rotl((BufSub[i] + x + y), 3);
                x = BufSub[i];
                i = (i + 1) % RC5_SUB;
                BufKey[j] = rotl((BufKey[j] + x + y), (x + y));
                y = BufKey[j];
                j = (j + 1) % RC5_KEY;
            }
        }

        /// <summary>
        /// Encrypts data with the CORC5 algorithm.
        /// </summary>
        public void Encrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0 && Length % 8 == 0);

            Length = (Length / 8) * 8;

            var pBufData = (UInt32*)pBuf;
            for (var k = 0; k < Length / 8; k++)
            {
                var a = pBufData[2 * k];
                var b = pBufData[2 * k + 1];

                var le = a + BufSub[0];
                var re = b + BufSub[1];
                for (var i = 1; i <= RC5_12; i++)
                {
                    le = rotl((le ^ re), re) + BufSub[2 * i];
                    re = rotl((re ^ le), le) + BufSub[2 * i + 1];
                }

                pBufData[2 * k] = le;
                pBufData[2 * k + 1] = re;
            }
        }

        /// <summary>
        /// Decrypts data with the CORC5 algorithm.
        /// </summary>
        public void Decrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0 && Length % 8 == 0);

            Length = (Length / 8) * 8;

            var pBufData = (UInt32*)pBuf;
            for (var k = 0; k < Length / 8; k++)
            {
                var ld = pBufData[2 * k];
                var rd = pBufData[2 * k + 1];
                for (var i = RC5_12; i >= 1; i--)
                {
                    rd = rotr((rd - BufSub[2 * i + 1]), ld) ^ ld;
                    ld = rotr((ld - BufSub[2 * i]), rd) ^ rd;
                }

                var b = rd - BufSub[1];
                var a = ld - BufSub[0];

                pBufData[2 * k] = a;
                pBufData[2 * k + 1] = b;
            }
        }

        /// <summary>
        /// Generates a random key (Key) to use for the algorithm.
        /// CO2: { 0x3C, 0xDC, 0xFE, 0xE8, 0xC4, 0x54, 0xD6, 0x7E, 0x16, 0xA6, 0xF8, 0x1A, 0xE8, 0xD0, 0x38, 0xBE }
        /// </summary>
        public void GenerateKey(Byte[] BufKey)
        {
            Kernel.assert(BufKey != null);
            Kernel.assert(BufKey.Length > 0 && BufKey.Length == RC5_16);

            var Length = BufKey.Length;
            fixed (Byte* pBufKey = BufKey)
                GenerateKey(pBufKey, Length);
        }

        /// <summary>
        /// Encrypts data with the CORC5 algorithm.
        /// </summary>
        public void Encrypt(ref Byte[] Buf)
        {
            Kernel.assert(Buf != null);
            Kernel.assert(Buf.Length > 0 && Buf.Length % 8 == 0);

            var Length = Buf.Length;
            fixed (Byte* pBuf = Buf)
                Encrypt(pBuf, Length);
        }

        /// <summary>
        /// Decrypts data with the CORC5 algorithm.
        /// </summary>
        public void Decrypt(ref Byte[] Buf)
        {
            Kernel.assert(Buf != null);
            Kernel.assert(Buf.Length > 0 && Buf.Length % 8 == 0);

            var Length = Buf.Length;
            fixed (Byte* pBuf = Buf)
                Decrypt(pBuf, Length);
        }

        private UInt32 rotl(UInt32 Value, UInt32 Count)
        {
            Count %= 32;

            var High = Value >> (32 - (Int32)Count);
            return (Value << (Int32)Count) | High;
        }

        private UInt32 rotr(UInt32 Value, UInt32 Count)
        {
            Count %= 32;

            var Low = Value << (32 - (Int32)Count);
            return (Value >> (Int32)Count) | Low;
        }
    }
}

// * ************************************************************
// * * END:                                            corc5.cs *
// * ************************************************************