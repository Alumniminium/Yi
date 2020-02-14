using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL
{
    public unsafe class cstdlib
    {
        #region Constants
        /// <summary>
        /// A system-dependent integral expression that, when used as the argument for function exit,
        /// should signify that the application failed.
        /// </summary>
        public const Int32 EXIT_FAILURE = 1;

        /// <summary>
        /// A system-dependent integral expression that, when used as the argument for function exit,
        /// should signify that the application was successful.
        /// </summary>
        public const Int32 EXIT_SUCCESS = 0;

        /// <summary>
        /// An integral constant expression whose value is the maximum value returned by the rand function.
        /// </summary>
        public const Int32 RAND_MAX = 0x7FFF;

        /// <summary>
        /// A null pointer constant.
        /// </summary>
        public static readonly void* NULL = (void*)0;
        #endregion

        #region Structures
        /// <summary>
        /// Structure used to represent the value of an integral division performed by div.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct div_t
        {
            /// <summary>
            /// Represents the quotient of the integral division operation performed by div, which is the
            /// integer of lesser magnitude that is neares to the algebraic quotient.
            /// </summary>
            public Int32 quot;

            /// <summary>
            /// Represents the remainder of the integral division operation performed by div, which is the
            /// integer resulting from subtracting quot to the numerator of the operation.
            /// </summary>
            public Int32 rem;
        }

        /// <summary>
        /// Structure used to represent the value of an integral division performed by div or ldiv.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ldiv_t
        {
            /// <summary>
            /// Represents the quotient of the integral division operation performed by div, which is the
            /// integer of lesser magnitude that is neares to the algebraic quotient.
            /// </summary>
            public Int64 quot;

            /// <summary>
            /// Represents the remainder of the integral division operation performed by div, which is the
            /// integer resulting from subtracting quot to the numerator of the operation.
            /// </summary>
            public Int64 rem;
        }

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
        /// Aborts the process with an abnormal program termination.
        /// 
        /// The function generates the SIGABRT signal, which by default causes the program to terminate returning an
        /// unsuccessful termination error code to the host environment.
        /// </summary>
        public static void abort()
        {
            //TODO: Check for a valid abort() implementation.
            Environment.FailFast("Abort call! SIGABRT signal.");
        }

        /// <summary>
        /// Returns the absolute value of parameter n.
        /// </summary>
        /// <param name="n">Integral value.</param>
        /// <returns>The absolute value of n.</returns>
        public static Int32 abs(Int32 n)
        {
            return Math.Abs(n);
        }

        /// <summary>
        /// Returns the absolute value of parameter n.
        /// </summary>
        /// <param name="n">Integral value.</param>
        /// <returns>The absolute value of n.</returns>
        public static Int64 abs(Int64 n)
        {
            return Math.Abs(n);
        }

        /// <summary>
        /// The function pointed by the function pointer argument is called when the program terminates normally.
        /// 
        /// If more than one atexit function has been specified by different calls to this function, they are all
        /// executed in reverse order as a stack, i.e. the last function specified is the first to be executed at exit.
        ///
        /// One single function can be registered to be executed at exit more than once.
        /// 
        /// </summary>
        /// <param name="function">Function to be called. The function has to return no value and accept no arguments.</param>
        /// <returns>
        /// A zero value is returned if the function was successfully registered, or a non-zero value if it failed.
        /// </returns>
        public static Int32 atexit(void* function)
        {
            //TODO: Check for a C# equivalence.
            return 1;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as a floating point number and returns its value as a double.
        /// </summary>
        /// <param name="str">C string beginning with the representation of a floating-point number.</param>
        /// <returns>
        /// On success, the function returns the converted floating point number as a double value.
        /// If no valid conversion could be performed, or if the correct value would cause underflow, a zero value (0.0) is returned.
        /// If the correct value is out of the range of representable values, a positive or negative HUGE_VAL is returned.
        /// </returns>
        public static Double atof(String str)
        {
            var value = 0.0;
            if (!Double.TryParse(str, out value))
                return Double.PositiveInfinity;
            return value;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as a floating point number and returns its value as a double.
        /// </summary>
        /// <param name="str">C string beginning with the representation of a floating-point number.</param>
        /// <returns>
        /// On success, the function returns the converted floating point number as a double value.
        /// If no valid conversion could be performed, or if the correct value would cause underflow, a zero value (0.0) is returned.
        /// If the correct value is out of the range of representable values, a positive or negative HUGE_VAL is returned.
        /// </returns>
        public static Double atof(Byte* str)
        {
            var value = 0.0;
            if (!Double.TryParse(new String((SByte*)str), out value))
                return Double.PositiveInfinity;
            return value;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as an integral number, which is returned as an int value.
        /// </summary>
        /// <param name="str">C string beginning with the representation of an integral number.</param>
        /// <returns>
        /// On success, the function returns the converted integral number as an int value.
        /// If no valid conversion could be performed, a zero value is returned.
        /// If the correct value is out of the range of representable values, INT_MAX or INT_MIN is returned.
        /// </returns>
        public static Int32 atoi(String str)
        {
            var value = 0;
            if (!Int32.TryParse(str, out value))
                return Int32.MaxValue;
            return value;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as an integral number, which is returned as an int value.
        /// </summary>
        /// <param name="str">C string beginning with the representation of an integral number.</param>
        /// <returns>
        /// On success, the function returns the converted integral number as an int value.
        /// If no valid conversion could be performed, a zero value is returned.
        /// If the correct value is out of the range of representable values, INT_MAX or INT_MIN is returned.
        /// </returns>
        public static Int32 atoi(Byte* str)
        {
            var value = 0;
            if (!Int32.TryParse(new String((SByte*)str), out value))
                return Int32.MaxValue;
            return value;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as an integral number, which is returned as a long int value.
        /// </summary>
        /// <param name="str">C string beginning with the representation of an integral number.</param>
        /// <returns>
        /// On success, the function returns the converted integral number as a long int value.
        /// If no valid conversion could be performed, a zero value is returned.
        /// If the correct value is out of the range of representable values, LONG_MAX or LONG_MIN is returned.
        /// </returns>
        public static Int64 atol(String str)
        {
            Int64 value = 0;
            if (!Int64.TryParse(str, out value))
                return Int64.MaxValue;
            return value;
        }

        /// <summary>
        /// Parses the C string str interpreting its content as an integral number, which is returned as a long int value.
        /// </summary>
        /// <param name="str">C string beginning with the representation of an integral number.</param>
        /// <returns>
        /// On success, the function returns the converted integral number as a long int value.
        /// If no valid conversion could be performed, a zero value is returned.
        /// If the correct value is out of the range of representable values, LONG_MAX or LONG_MIN is returned.
        /// </returns>
        public static Int64 atol(Byte* str)
        {
            Int64 value = 0;
            if (!Int64.TryParse(new String((SByte*)str), out value))
                return Int64.MaxValue;
            return value;
        }

        public static void bsearch()
        {
            //TODO: Implement the binary search...

            //Array.BinarySearch
            //public void* bsearch(void* key, void* _base, size_t num, size_t size, int (*comparator)(void*, void*));
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allocates a block of memory for an array of num elements, each of them size bytes long, and initializes
        /// all its bits to zero.
        /// 
        /// The effective result is the allocation of an zero-initialized memory block of (num * size) bytes.
        /// </summary>
        /// <param name="num">Number of elements to be allocated.</param>
        /// <param name="size">Size of elements.</param>
        /// <returns>
        /// A pointer to the memory block allocated by the function.
        /// The type of this pointer is always void*, which can be cast to the desired type of data pointer in
        /// order to be dereferenceable.
        /// If the function failed to allocate the requested block of memory, a NULL pointer is returned.
        /// </returns>
        public static void* calloc(size_t num, size_t size)
        {
            var ptr = NULL;
            try { ptr = Marshal.AllocHGlobal(size.value * num.value).ToPointer(); }
            catch { ptr = NULL; }

            //The use of Int32 on both x86 and x64 is the best solution to upgrade the speed.
            //It may be due to the aligment of the data.

            //Considering that under Windows, any integer with all its bits to zero is equal to zero.
            //It is not the real implementation, but an optimized one for any OS running the .net/mono framework.

            if (ptr != NULL)
            {
                var len = size.value * num.value;

                var count = len / sizeof(Int32);
                for (var i = 0; i < count; i++)
                    *(((Int32*)ptr) + i) = 0;

                var pos = len - (len % sizeof(Int32));
                for (var i = 0; i < size % sizeof(Int32); i++)
                    *(((Byte*)ptr) + pos + i) = 0;
            }
            return ptr;
        }

        /// <summary>
        /// Returns the integral quotient and remainder of the division of numerator by denominator as a
        /// structure of type div_t, which has two members: quot and rem.
        /// </summary>
        /// <param name="numerator">Numerator.</param>
        /// <param name="denominator">Denominator.</param>
        /// <returns>
        /// The result is returned by value in a structure defined in <cstdlib>, which has two members.
        /// </returns>
        public static div_t div(Int32 numerator, Int32 denominator)
        {
            var result = new div_t();
            result.quot = numerator / denominator;
            result.rem = numerator % denominator;
            return result;
        }

        /// <summary>
        /// Returns the integral quotient and remainder of the division of numerator by denominator as a
        /// structure of type ldiv_t, which has two members: quot and rem.
        /// </summary>
        /// <param name="numerator">Numerator.</param>
        /// <param name="denominator">Denominator.</param>
        /// <returns>
        /// The result is returned by value in a structure defined in <cstdlib>, which has two members.
        /// </returns>
        public static ldiv_t div(Int64 numerator, Int64 denominator)
        {
            var result = new ldiv_t();
            result.quot = numerator / denominator;
            result.rem = numerator % denominator;
            return result;
        }

        /// <summary>
        /// Terminates the process normally, performing the regular cleanup for terminating processes.
        /// </summary>
        /// <param name="status">Status value returned to the parent process.</param>
        public static void exit(Int32 status)
        {
            Environment.Exit(status);
        }

        /// <summary>
        /// A block of memory previously allocated using a call to malloc, calloc or realloc is deallocated,
        /// making it available again for further allocations.
        /// 
        /// Notice that this function leaves the value of ptr unchanged, hence it still points to the same
        /// (now invalid) location, and not to the null pointer.
        /// </summary>
        /// <param name="ptr">Pointer to a memory block previously allocated to be deallocated.</param>
        public static void free(void* ptr)
        {
            if (ptr != null && ptr != NULL)
                Marshal.FreeHGlobal((IntPtr)ptr);
        }

        public static String getenv(String name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        #endregion
    }
}
