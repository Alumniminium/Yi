// * ************************************************************
// * * START:                                          cofac.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Conquer Online File Asymmetric Cipher for the library.
// * cofac.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (December 29th, 2011)
// * Copyright (C) 2011 CptSky
// *
// * ************************************************************
// *                      SPECIAL THANKS
// * ************************************************************
// * high6 (high6 @ e*pvp)
// * 
// * ************************************************************

using System;

namespace CO2_CORE_DLL.Security.Cryptography
{
    /// <summary>
    /// Conquer Online File Asymmetric Cipher
    /// </summary>
    public unsafe class COFAC
    {
        private const Int32 COFAC_KEY = 128;

        private Byte* BufKey = null;

        /// <summary>
        /// Create a new COFAC instance.
        /// </summary>
        public COFAC() { }

        ~COFAC()
        {
            if (BufKey != null)
                Kernel.free(BufKey);
        }

        /// <summary>
        /// Generates a key (Key) to use for the algorithm.
        /// </summary>
        public void GenerateKey(UInt32 Seed)
        {
            if (BufKey != null)
                Kernel.free(BufKey);

            BufKey = (Byte*)Kernel.malloc(COFAC_KEY);

            var Rand = new MSRandom(Seed);
            for (var i = 0; i < COFAC_KEY; i++)
                BufKey[i] = (Byte)(Rand.Next() % 0x100);
        }

        /// <summary>
        /// Encrypts data with the COFAC algorithm.
        /// </summary>
        public void Encrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0);

            for (var i = 0; i < Length; i++)
            {
                Int32 tmp = (Byte)((pBuf[i] >> (8 - (i % 8))) + (pBuf[i] << (i % 8)));
                pBuf[i] = (Byte)(tmp ^ BufKey[i % COFAC_KEY]);
            }
        }

        /// <summary>
        /// Decrypts data with the COFAC algorithm.
        /// </summary>
        public void Decrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0);

            for (var i = 0; i < Length; i++)
            {
                var tmp = pBuf[i] ^ BufKey[i % COFAC_KEY];
                pBuf[i] = (Byte)((tmp << (8 - (i % 8))) + (tmp >> (i % 8)));
            }
        }

        /// <summary>
        /// Encrypts data with the COFAC algorithm.
        /// </summary>
        public void Encrypt(ref Byte[] Buf)
        {
            Kernel.assert(Buf != null);
            Kernel.assert(Buf.Length > 0);

            var Length = Buf.Length;
            fixed (Byte* pBuf = Buf)
                Encrypt(pBuf, Length);
        }

        /// <summary>
        /// Decrypts data with the COFAC algorithm.
        /// </summary>
        public void Decrypt(ref Byte[] Buf)
        {
            Kernel.assert(Buf != null);
            Kernel.assert(Buf.Length > 0);

            var Length = Buf.Length;
            fixed (Byte* pBuf = Buf)
                Decrypt(pBuf, Length);
        }
    }
}

// * ************************************************************
// * * END:                                            cofac.cs *
// * ************************************************************