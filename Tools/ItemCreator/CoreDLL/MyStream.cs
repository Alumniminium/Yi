// * ************************************************************
// * * START:                                       mystream.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Stream extension for the library.
// * mystream.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (May 20th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************

using System.IO;

namespace ItemCreator.CoreDLL
{
    public static unsafe class MyStream
    {
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within 
        /// the stream by the number of bytes read.
        /// </summary>
        public static int Read(this Stream Stream, void* pBuf, int Size)
        {
            var Buffer = new byte[Size];
            var Read = Stream.Read(Buffer, 0, Size);
            Kernel.memcpy(pBuf, Buffer, Read);
            return Read;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the position within 
        /// the stream by the number of bytes written.
        /// </summary>
        public static void Write(this Stream Stream, void* pBuf, int Size)
        {
            var Buffer = new byte[Size];
            Kernel.memcpy(Buffer, pBuf, Size);
            Stream.Write(Buffer, 0, Size);
        }
    }
}

// * ************************************************************
// * * END:                                         mystream.cs *
// * ************************************************************