// * ************************************************************
// * * START:                                          cosac.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Conquer Online Server Asymmetric Cipher for the library.
// * cosac.cs
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
// *                      SPECIAL THANKS
// * ************************************************************
// * Sparkie (unknownone @ e*pvp)
// * 
// * ************************************************************

using System;

namespace CO2_CORE_DLL.Security.Cryptography
{
    /// <summary>
    /// Conquer Online Server Asymmetric Cipher
    /// </summary>
    public unsafe class COSAC
    {
        private const Int32 COSAC_IV = 512;
        private const Int32 COSAC_KEY = 512;

        private Byte* BufIV = null;
        private Byte* BufKey = null;
        private UInt16 EncryptCounter = 0;
        private UInt16 DecryptCounter = 0;

        /// <summary>
        /// Create a new COSAC instance.
        /// </summary>
        public COSAC() { }

        ~COSAC()
        {
            if (BufIV != null)
                Kernel.free(BufIV);
            if (BufKey != null)
                Kernel.free(BufKey);
        }

        /// <summary>
        /// Generates an initialization vector (IV) to use for the algorithm.
        /// CO2(P: 0x13FA0F9D, G: 0x6D5C7962)
        /// </summary>
        public void GenerateIV(Int32 P, Int32 G)
        {
            if (BufIV != null)
                Kernel.free(BufIV);

            BufIV = (Byte*)Kernel.malloc(COSAC_IV);
            Int16 K = COSAC_IV / 2;

            var pBufPKey = (Byte*)&P;
            var pBufGKey = (Byte*)&G;

            for (Int16 i = 0; i < K; i++)
            {
                BufIV[i + 0] = pBufPKey[0];
                BufIV[i + K] = pBufGKey[0];
                pBufPKey[0] = (Byte)((pBufPKey[1] + (Byte)(pBufPKey[0] * pBufPKey[2])) * pBufPKey[0] + pBufPKey[3]);
                pBufGKey[0] = (Byte)((pBufGKey[1] - (Byte)(pBufGKey[0] * pBufGKey[2])) * pBufGKey[0] + pBufGKey[3]);
            }
        }

        /// <summary>
        /// Generates a key (Key) to use for the algorithm and reset the encrypt counter.
        /// In Conquer Online: A = Token, B = AccountUID
        /// </summary>
        public void GenerateKey(Int32 A, Int32 B)
        {
            Kernel.assert(BufIV != null);
 
            if (BufKey != null)
                Kernel.free(BufKey);

            BufKey = (Byte*)Kernel.malloc(COSAC_KEY);
            Int16 K = COSAC_KEY / 2;

            //UInt32 tmp1 = 0;
            //tmp1 = (UInt32)(A + B);

            //Byte* tmpKey1 = (Byte*)&tmp1;
            //((Int16*)tmpKey1)[0] ^= 0x4321;

            //for (SByte i = 0; i < 4; i++)
            //    tmpKey1[3 - i] ^= (Byte)(A >> (24 - (8 * i)));

            var tmp1 = (UInt32)(((A + B) ^ 0x4321) ^ A);
            var tmp2 = tmp1 * tmp1;

            var tmpKey1 = (Byte*)&tmp1;
            var tmpKey2 = (Byte*)&tmp2;
            for (Int16 i = 0; i < K; i++)
            {
                BufKey[i + 0] = (Byte)(BufIV[i + 0] ^ tmpKey1[(i % 4)]);
                BufKey[i + K] = (Byte)(BufIV[i + K] ^ tmpKey2[(i % 4)]);
            }
            EncryptCounter = 0;
        }

        /// <summary>
        /// Encrypts data with the COSAC algorithm.
        /// </summary>
        public void Encrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0);

            Int16 K = COSAC_IV / 2;
            for (var i = 0; i < Length; i++)
            {
                pBuf[i] ^= (Byte)0xAB;
                pBuf[i] = (Byte)(pBuf[i] >> 4 | pBuf[i] << 4);
                if (BufIV != null)
                {
                    pBuf[i] ^= (Byte)(BufIV[(Byte)(EncryptCounter & 0xFF) + 0]);
                    pBuf[i] ^= (Byte)(BufIV[(Byte)(EncryptCounter >> 8) + K]);
                }
                EncryptCounter++;
            }
        }

        /// <summary>
        /// Decrypts data with the COSAC algorithm.
        /// </summary>
        public void Decrypt(Byte* pBuf, Int32 Length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(Length > 0);

            Int16 K = COSAC_IV / 2;
            if (BufKey != null)
                K = COSAC_KEY / 2;

            for (var i = 0; i < Length; i++)
            {
                pBuf[i] ^= (Byte)0xAB;
                pBuf[i] = (Byte)(pBuf[i] >> 4 | pBuf[i] << 4);
                if (BufKey != null)
                {
                    pBuf[i] ^= (Byte)(BufKey[(Byte)(DecryptCounter & 0xFF) + 0]);
                    pBuf[i] ^= (Byte)(BufKey[(Byte)(DecryptCounter >> 8) + K]);
                }
                else if (BufIV != null)
                {
                    pBuf[i] ^= (Byte)(BufIV[(Byte)(DecryptCounter & 0xFF) + 0]);
                    pBuf[i] ^= (Byte)(BufIV[(Byte)(DecryptCounter >> 8) + K]);
                }
                DecryptCounter++;
            }
        }

        /// <summary>
        /// Encrypts data with the COSAC algorithm.
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
        /// Decrypts data with the COSAC algorithm.
        /// </summary>
        public void Decrypt(ref Byte[] Buf)
        {
            Kernel.assert(Buf != null);
            Kernel.assert(Buf.Length > 0);

            var Length = Buf.Length;
            fixed (Byte* pBuf = Buf)
                Decrypt(pBuf, Length);
        }

        /// <summary>
        /// Resets the decrypt and the encrypt counters.
        /// </summary>
        public void ResetCounters() { DecryptCounter = 0; EncryptCounter = 0; }
    }
}

// * ************************************************************
// * * END:                                            cosac.cs *
// * ************************************************************