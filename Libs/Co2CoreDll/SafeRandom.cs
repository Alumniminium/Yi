// * ************************************************************
// * * START:                                     saferandom.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Thread-safe random class for the library.
// * saferandom.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by nTL3fTy
// * Copyright (C) 2012 nTL3fTy
// *
// * ************************************************************

using System;
using System.Threading;

namespace CO2_CORE_DLL
{
    public class SafeRandom : Random
    {
        private Object _syncRoot;

        public Object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                return _syncRoot;
            }
        }

        /// <summary>
        /// Initializes a new instance using a time-dependant default seed value.
        /// </summary>
        public SafeRandom() : base() { }

        /// <summary>
        /// Initializes a new instance using the specified seed value.
        /// </summary>
        public SafeRandom(Int32 Seed) : base(Seed) { }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        protected override Double Sample() { lock (SyncRoot) { return base.Sample(); } }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        public override Int32 Next() { lock (SyncRoot) { return base.Next(); } }

        //Returns a nonnegative random number less than the specified maximum.
        public override Int32 Next(Int32 maxValue) { lock (SyncRoot) { return base.Next(maxValue); } }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        public override Int32 Next(Int32 minValue, Int32 maxValue) { lock (SyncRoot) { return base.Next(minValue, maxValue); } }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        public override void NextBytes(Byte[] buffer) { lock (SyncRoot) { base.NextBytes(buffer); } }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        public override Double NextDouble() { lock (SyncRoot) { return base.NextDouble(); } }
    }
}

// * ************************************************************
// * * END:                                       saferandom.cs *
// * ************************************************************