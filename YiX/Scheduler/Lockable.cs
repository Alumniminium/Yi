using System.ComponentModel;
using System.Threading;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal class Lockable
    {
        #region Properties

        protected int Lock;

        #endregion

        /// <summary>
        ///     Returns the number of lock conflicts that have occurred
        /// </summary>
        public static long Conflicts => _conflicts;
        private const int SpinCycles = 100;
        private static long _conflicts;

        /// <summary>
        ///     Aquire the lock
        /// </summary>
        protected void AquireLock()
        {
            // Assume that we will grab the lock - call CompareExchange
            if (Interlocked.CompareExchange(ref Lock, 1, 0) != 1)
                return;
            var n = 0;

            // Could not grab the lock - spin/wait until the lock looks obtainable
            while (Lock == 1)
            {
                if (n++ <= SpinCycles)
                    continue;
                Interlocked.Increment(ref _conflicts);
                n = 0;
                Thread.Sleep(0);
            }

            // Try to grab the lock - call CompareExchange
            while (Interlocked.CompareExchange(ref Lock, 1, 0) == 1)
            {
                n = 0;

                // Someone else grabbed the lock.  Continue to spin/wait until the lock looks obtainable
                while (Lock == 1)
                {
                    if (n++ <= SpinCycles)
                        continue;
                    Interlocked.Increment(ref _conflicts);
                    n = 0;
                    Thread.Sleep(0);
                }
            }
        }

        /// <summary>
        ///     Release the lock
        /// </summary>
        protected void ReleaseLock()
        {
            Lock = 0;
        }
    }
}