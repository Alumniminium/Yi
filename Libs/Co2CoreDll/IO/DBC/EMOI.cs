// * ************************************************************
// * * START:                                           emoi.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * EMOI class for the library.
// * emoi.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (January 22th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.IO.DBC
{
    /// <summary>
    /// DBC / EMOI
    /// Files: EmotionIco
    /// </summary>
    public unsafe class EMOI
    {
        public const Int32 MAX_NAMESIZE = 0x20;

        private const Int32 EMOI_IDENTIFIER = 0x494F4D45;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public Int32 Identifier;
            public Int32 Amount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public Int32 ID;
            public fixed Byte Name[MAX_NAMESIZE];
        };

        private Dictionary<Int32, IntPtr> Entries = null;

        /// <summary>
        /// Create a new EMOI instance to handle the TQ's EMOI file.
        /// </summary>
        public EMOI()
        {
            this.Entries = new Dictionary<Int32, IntPtr>();
        }

        ~EMOI()
        {
            Clear();
        }

        /// <summary>
        /// Reset the dictionary and free all the used memory.
        /// </summary>
        public void Clear()
        {
            if (Entries != null)
            {
                lock (Entries)
                {
                    foreach (var Ptr in Entries.Values)
                        Kernel.free((Entry*)Ptr);
                }
                Entries.Clear();
            }
        }

        /// <summary>
        /// Load the specified EMOI file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(String Path)
        {
            Clear();

            lock (Entries)
            {
                using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    Header* pHeader = stackalloc Header[1];

                    Stream.Read(Buffer, 0, sizeof(Header));
                    Kernel.memcpy(pHeader, Buffer, sizeof(Header));

                    if (pHeader->Identifier != EMOI_IDENTIFIER)
                        throw new Exception("Invalid EMOI Header in file: " + Path);

                    for (var i = 0; i < pHeader->Amount; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                        Stream.Read(Buffer, 0, sizeof(Entry));
                        Kernel.memcpy(pEntry, Buffer, sizeof(Entry));

                        if (!Entries.ContainsKey(pEntry->ID))
                            Entries.Add(pEntry->ID, (IntPtr)pEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified EMOI file (in plain format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Entries)
            {
                using (var Stream = new StreamReader(Path, Encoding.GetEncoding("Windows-1252")))
                {
                    String Line = null;
                    var LineC = 0;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        LineC++;

                        var Parts = Line.Split(' ');
                        var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));

                        try
                        {
                            pEntry->ID = Int32.Parse(Parts[0]);

                            Byte* pName = stackalloc Byte[Parts[1].Length + 1];
                            Parts[1].ToPointer(pName);
                            Kernel.memcpy(pEntry->Name, pName, Math.Min(MAX_NAMESIZE - 1, Kernel.strlen(pName)));

                            if (!Entries.ContainsKey(pEntry->ID))
                                Entries.Add(pEntry->ID, (IntPtr)pEntry);
                        }
                        catch (Exception Exc)
                        {
                            Console.WriteLine("Error at line {0}.\n{1}", LineC, Exc);
                            Kernel.free(pEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified EMOI file (in binary format).
        /// </summary>
        public void SaveToDat(String Path)
        {
            using (var Stream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                var Pointers = new IntPtr[0];

                lock (Entries)
                {
                    Pointers = new IntPtr[Entries.Count];
                    Entries.Values.CopyTo(Pointers, 0);
                }

                Header* pHeader = stackalloc Header[1];
                pHeader->Identifier = EMOI_IDENTIFIER;
                pHeader->Amount = Pointers.Length;

                Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                Stream.Write(Buffer, 0, sizeof(Header));

                for (var i = 0; i < Pointers.Length; i++)
                {
                    Kernel.memcpy(Buffer, (Entry*)Pointers[i], sizeof(Entry));
                    Stream.Write(Buffer, 0, sizeof(Entry));
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified EMOI file (in plain format).
        /// </summary>
        public void SaveToTxt(String Path)
        {
            using (var Stream = new StreamWriter(Path, false, Encoding.GetEncoding("Windows-1252")))
            {
                var Pointers = new IntPtr[0];

                lock (Entries)
                {
                    Pointers = new IntPtr[Entries.Count];
                    Entries.Values.CopyTo(Pointers, 0);
                }

                for (var i = 0; i < Pointers.Length; i++)
                {
                    var pEntry = (Entry*)Pointers[i];

                    var Builder = new StringBuilder(Kernel.MAX_BUFFER_SIZE);
                    Builder.Append(pEntry->ID + " ");
                    Builder.Append(Kernel.cstring(pEntry->Name, MAX_NAMESIZE));
                    Stream.WriteLine(Builder.ToString());
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                             emoi.cs *
// * ************************************************************