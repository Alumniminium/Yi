using System;

namespace CO2_CORE_DLL
{
    public unsafe class cassert
    {
        #region Functions
        //TODO: MsgBox -> Assertion failed: expression, file filename, line line number 

        /// <summary>
        /// If the argument expression of this macro with functional form compares equal to zero (false), a message is
        /// written to the standard error device and abort is called, terminating the program execution.
        /// </summary>
        /// <param name="expression">If this expression evaluates to 0, this causes an assertion failure.</param>
        public static void assert(Boolean expression)
        {
            #if !NDEBUG
            if (!expression)
            {
                throw new Exception("Assertion failed!");
                cstdlib.abort();
            }
            #endif
        }

        /// <summary>
        /// If the argument expression of this macro with functional form compares equal to zero (false), a message is
        /// written to the standard error device and abort is called, terminating the program execution.
        /// </summary>
        /// <param name="expression">If this expression evaluates to 0, this causes an assertion failure.</param>
        public static void assert(Int32 expression)
        {
            #if !NDEBUG
            if (expression == 0)
            {
                throw new Exception("Assertion failed!");
                cstdlib.abort();
            }
            #endif
        }
        #endregion
    }
}
