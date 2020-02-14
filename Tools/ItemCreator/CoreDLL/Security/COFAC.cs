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

namespace ItemCreator.CoreDLL.Security
{
    /// <summary>
    /// Conquer Online File Asymmetric Cipher
    /// </summary>
    public unsafe class Cofac
    {
        private const int COFAC_KEY = 128;

        private byte* _bufKey = null;

        ~Cofac()
        {
            if (_bufKey != null)
                Kernel.free(_bufKey);
        }

        /// <summary>
        /// Generates a key (Key) to use for the algorithm.
        /// </summary>
        public void GenerateKey(uint seed)
        {
            if (_bufKey != null)
                Kernel.free(_bufKey);

            _bufKey = (byte*)Kernel.malloc(COFAC_KEY);

            var rand = new MSRandom(seed);
            for (var i = 0; i < COFAC_KEY; i++)
                _bufKey[i] = (byte)(rand.Next() % 0x100);
        }

        /// <summary>
        /// Encrypts data with the COFAC algorithm.
        /// </summary>
        public void Encrypt(byte* pBuf, int length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(length > 0);

            for (var i = 0; i < length; i++)
            {
                if (pBuf == null) continue;
                int tmp = (byte)((pBuf[i] >> (8 - i % 8)) + (pBuf[i] << (i % 8)));
                pBuf[i] = (byte)(tmp ^ _bufKey[i % COFAC_KEY]);
            }
        }

        /// <summary>
        /// Decrypts data with the COFAC algorithm.
        /// </summary>
        public void Decrypt(byte* pBuf, int length)
        {
            Kernel.assert(pBuf != null);
            Kernel.assert(length > 0);

            for (var i = 0; i < length; i++)
            {
                if (pBuf == null) continue;
                var tmp = pBuf[i] ^ _bufKey[i % COFAC_KEY];
                pBuf[i] = (byte)((tmp << (8 - i % 8)) + (tmp >> (i % 8)));
            }
        }

        /// <summary>
        /// Encrypts data with the COFAC algorithm.
        /// </summary>
        public void Encrypt(ref byte[] buf)
        {
            Kernel.assert(buf != null);
            Kernel.assert(buf != null && buf.Length > 0);

            if (buf == null) return;
            var length = buf.Length;
            fixed (byte* pBuf = buf)
                Encrypt(pBuf, length);
        }

        /// <summary>
        /// Decrypts data with the COFAC algorithm.
        /// </summary>
        public void Decrypt(ref byte[] buf)
        {
            Kernel.assert(buf != null);
            Kernel.assert(buf != null && buf.Length > 0);

            if (buf == null) return;
            var length = buf.Length;
            fixed (byte* pBuf = buf)
                Decrypt(pBuf, length);
        }
    }
}

// * ************************************************************
// * * END:                                            cofac.cs *
// * ************************************************************