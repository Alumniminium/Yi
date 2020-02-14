// * ************************************************************
// * * START:                                      autoallot.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * AutoAllot class for the library.
// * autoallot.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ItemCreator.CoreDLL.IO.Ini
{
    /// <summary>
    /// AutoAllot
    /// </summary>
    public unsafe class AutoAllot
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public readonly int Amount;
            public readonly int Level;
            public fixed int Professions[1]; //Profession / 10
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public readonly int Strength;
            public readonly int Agility;
            public readonly int Vitality;
            public readonly int Spirit;
        }

        //Entries[Profession][Level] = Stat;
        private Dictionary<int, IntPtr[]> _entries;

        /// <summary>
        /// Create a new LevelExp instance to handle the TQ's AutoAllot file.
        /// </summary>
        public AutoAllot()
        {
            _entries = new Dictionary<int, IntPtr[]>();
        }

        ~AutoAllot()
        {
            Clear();
        }

        /// <summary>
        /// Reset the dictionary and free all the used memory.
        /// </summary>
        public void Clear()
        {
            lock (_entries)
            {
                foreach (var value in _entries.Values)
                {
                    foreach (var t in value)
                        Kernel.free((void*)t);
                }
                _entries.Clear();
            }
        }

        /// <summary>
        /// Load the specified autoallot file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(string path)
        {
            Clear();

            lock (_entries)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                    stream.Read(buffer, 0, sizeof(int));

                    int amount;
                    fixed (byte* pBuffer = buffer)
                        amount = *(int*)pBuffer;
                    var length = (amount + 2) * sizeof(int);

                    var pHeader = (Header*)Kernel.malloc(length);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, length);
                    Kernel.memcpy(pHeader, buffer, length);

                    _entries = new Dictionary<int, IntPtr[]>(amount);
                    for (var i = 0; i < pHeader->Amount; i++)
                    {
                        _entries.Add(pHeader->Professions[i], new IntPtr[pHeader->Level]);
                        for (var j = 0; j < pHeader->Level; j++)
                        {
                            var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                            stream.Read(buffer, 0, sizeof(Entry));
                            Kernel.memcpy(pEntry, buffer, sizeof(Entry));

                            _entries[pHeader->Professions[i]][j] = (IntPtr)pEntry;
                        }
                    }
                    Kernel.free(pHeader);
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                        autoallot.cs *
// * ************************************************************