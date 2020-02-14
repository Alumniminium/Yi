using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL
{
    public unsafe class cstring
    {
        #region Constants
        /// <summary>
        /// A null pointer constant.
        /// </summary>
        public static readonly void* NULL = (void*)0;
        #endregion

        #region Structures
        /// <summary>
        /// size_t corresponds to the integer data type returned by the language operator sizeof and is defined
        /// in the <cstdlib> header file (among others) as an unsigned integer type.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct size_t
        {
            public Int32 value;

            public size_t(Int32 val) { value = val; }

            public static implicit operator Int32(size_t size)
            { return size.value; }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, void* src, size_t num)
        {
            //The use of Int32 on both x86 and x64 is the best solution to get the best speed.
            //It may be due to the aligment of the data.

            var amount = num / sizeof(size_t);
            for (var i = 0; i < amount; i++)
                ((size_t*)dest)[i] = ((size_t*)src)[i];

            amount = num % sizeof(size_t);

            var pos = num - amount;
            for (var i = 0; i < amount; i++)
                (((Byte*)dest) + pos)[i] = (((Byte*)src) + pos)[i];

            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Byte[] memcpy(Byte[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Int16[] memcpy(Int16[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Int32[] memcpy(Int32[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Int64[] memcpy(Int64[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Single[] memcpy(Single[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Double[] memcpy(Double[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static Char[] memcpy(Char[] dest, void* src, size_t num)
        {
            Marshal.Copy((IntPtr)src, dest, 0, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Byte[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Int16[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Int32[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Int64[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Single[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Double[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source directly to the memory block
        /// pointed by destination.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memcpy(void* dest, Char[] src, size_t num)
        {
            Marshal.Copy(src, 0, (IntPtr)dest, num);
            return dest;
        }

        /// <summary>
        /// Copies the values of num bytes from the location pointed by source to the memory block pointed by
        /// destination. Copying takes place as if an intermediate buffer were used, allowing the destination
        /// and source to overlap.
        /// 
        /// The underlying type of the objects pointed by both the source and destination pointers are irrelevant
        /// for this function; The result is a binary copy of the data.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">Pointer to the source of data to be copied.</param>
        /// <param name="num">Number of bytes to copy.</param>
        /// <returns>The destination is returned.</returns>
        public static void* memmove(void* dest, void* src, size_t num)
        {
            //TODO: Check for overlapping...
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the C string pointed by source into the array pointed by destination, including the
        /// terminating null character.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">C string to be copied.</param>
        /// <returns>The destination is returned.</returns>
        public static Byte* strcpy(Byte* dest, Byte* src)
        {
            for (var i = 0; i < Int32.MaxValue; i++)
            {
                dest[i] = src[i];
                if (src[i] == 0) //NUL character
                    break;
            }
            return dest;
        }

        /// <summary>
        /// Copies the C string pointed by source into the array pointed by destination, including the
        /// terminating null character.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">C string to be copied.</param>
        /// <returns>The destination is returned.</returns>
        public static Byte* strcpy(Byte* dest, String src)
        {
            var bytes = Encoding.Default.GetBytes(src);

            var i = 0;
            for (i = 0; i < bytes.Length; i++)
            {
                dest[i] = bytes[i];
                if (bytes[i] == 0) //NUL character
                    return dest;
            }
            dest[i] = 0;
            return dest;
        }

        /// <summary>
        /// Copies the C string pointed by source into the array pointed by destination, including the
        /// terminating null character.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">C string to be copied.</param>
        /// <returns>The destination is returned.</returns>
        public static String strcpy(ref String dest, Byte* src)
        {
            dest = new String((SByte*)src);
            return dest;
        }

        /// <summary>
        /// Copies the C string pointed by source into the array pointed by destination, including the
        /// terminating null character.
        /// </summary>
        /// <param name="dest">Pointer to the destination array where the content is to be copied.</param>
        /// <param name="src">C string to be copied.</param>
        /// <returns>The destination is returned.</returns>
        public static String strcpy(ref String dest, String src)
        {
            dest = (String)src.Clone();
            return dest;
        }


        #endregion
    }
}
