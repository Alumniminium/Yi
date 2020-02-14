// * ************************************************************
// * * START:                                       mystring.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * String extensions for the library.
// * mystring.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (January 17th, 2011)
// * Copyright (C) 2011 CptSky
// *
// * ************************************************************

using System.Text;

namespace ItemCreator.CoreDLL
{
    public static unsafe class MyString
    {
        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static byte* ToPointer(this string str)
        {
            var buffer = Encoding.GetEncoding("Windows-1252").GetBytes(str + "\0");
            var ptr = (byte*)Kernel.malloc(buffer.Length);

            fixed (byte* pBuffer = buffer)
                Kernel.memcpy(ptr, pBuffer, buffer.Length);
            return ptr;
        }

        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static byte* ToPointer(this string str, byte* ptr)
        {
            var buffer = Encoding.GetEncoding("Windows-1252").GetBytes(str + "\0");
            fixed (byte* pBuffer = buffer)
                Kernel.memcpy(ptr, pBuffer, buffer.Length);
            return ptr;
        }
    }
}

// * ************************************************************
// * * END:                                         mystring.cs *
// * ************************************************************