// * ************************************************************
// * * START:                                          ropt.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * ROPT class for the library.
// * ropt.cs
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
    /// DBC / ROPT
    /// Files: RolePart
    /// </summary>
    public unsafe class ROPT
    {
        public const Int32 MAX_NAMESIZE = 0x20;
        public const Int32 MAX_PATHSIZE = 0x100;

        private const Int32 ROPT_IDENTIFIER = 0x54504F52;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public Int32 Identifier;
            public Int32 PartAmount;
            public Int32 DumyAmount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Part
        {
            public fixed Byte Name[MAX_NAMESIZE];
            public fixed Byte MeshIni[MAX_PATHSIZE];
            public fixed Byte MotionIni[MAX_PATHSIZE];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Dumy
        {
            public Int32 UniqId;
            public fixed Byte Name[MAX_NAMESIZE];
        };

        private Dictionary<String, IntPtr> Parts = null;
        private Dictionary<Int32, IntPtr> Dumies = null;

        /// <summary>
        /// Create a new ROPT instance to handle the TQ's ROPT file.
        /// </summary>
        public ROPT()
        {
            this.Parts = new Dictionary<String, IntPtr>();
            this.Dumies = new Dictionary<Int32, IntPtr>();
        }

        ~ROPT()
        {
            Clear();
        }

        /// <summary>
        /// Reset the dictionary and free all the used memory.
        /// </summary>
        public void Clear()
        {
            if (Parts != null)
            {
                lock (Parts)
                {
                    foreach (var Ptr in Parts.Values)
                        Kernel.free((Part*)Ptr);
                }
                Parts.Clear();
            }
            if (Dumies != null)
            {
                lock (Dumies)
                {
                    foreach (var Ptr in Dumies.Values)
                        Kernel.free((Part*)Ptr);
                }
                Dumies.Clear();
            }
        }

        /// <summary>
        /// Load the specified ROPT file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(String Path)
        {
            Clear();

            lock (Parts)
            lock (Dumies)
            {
                using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    Header* pHeader = stackalloc Header[1];

                    Stream.Read(Buffer, 0, sizeof(Header));
                    Kernel.memcpy(pHeader, Buffer, sizeof(Header));

                    if (pHeader->Identifier != ROPT_IDENTIFIER)
                        throw new Exception("Invalid ROPT Header in file: " + Path);

                    for (var i = 0; i < pHeader->PartAmount; i++)
                    {
                        var pPart = (Part*)Kernel.malloc(sizeof(Part));
                        Stream.Read(Buffer, 0, sizeof(Part));
                        Kernel.memcpy(pPart, Buffer, sizeof(Part));

                        if (!Parts.ContainsKey(Kernel.cstring(pPart->Name, MAX_NAMESIZE)))
                            Parts.Add(Kernel.cstring(pPart->Name, MAX_NAMESIZE), (IntPtr)pPart);
                    }

                    for (var i = 0; i < pHeader->DumyAmount; i++)
                    {
                        var pDumy = (Dumy*)Kernel.malloc(sizeof(Dumy));
                        Stream.Read(Buffer, 0, sizeof(Dumy));
                        Kernel.memcpy(pDumy, Buffer, sizeof(Dumy));

                        if (!Dumies.ContainsKey(pDumy->UniqId))
                            Dumies.Add(pDumy->UniqId, (IntPtr)pDumy);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified ROPT file (in plain format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Parts)
            lock (Dumies)
            {
                var Ini = new Ini(Path);

                var PartAmount = Ini.ReadInt32("Config", "Count");
                var DumyAmount = Int32.Parse(Ini.ReadValue("Dumy", "Count").TrimEnd('¡'));

                Byte* pStr = stackalloc Byte[Kernel.MAX_BUFFER_SIZE];

                for (var i = 0; i < PartAmount; i++)
                {
                    var Name = Ini.ReadValue("Config", "Part" + i);
                    var MeshIni = Ini.ReadValue("Config", "MeshIni" + i);
                    var MotionIni = Ini.ReadValue("Config", "MotionIni" + i);

                    var pPart = (Part*)Kernel.calloc(sizeof(Part));

                    Name.ToPointer(pStr);
                    Kernel.memcpy(pPart->Name, pStr, Math.Min(MAX_NAMESIZE - 1, Kernel.strlen(pStr)));
                    MeshIni.ToPointer(pStr);
                    Kernel.memcpy(pPart->MeshIni, pStr, Math.Min(MAX_PATHSIZE - 1, Kernel.strlen(pStr)));
                    MotionIni.ToPointer(pStr);
                    Kernel.memcpy(pPart->MotionIni, pStr, Math.Min(MAX_PATHSIZE - 1, Kernel.strlen(pStr)));

                    if (!Parts.ContainsKey(Kernel.cstring(pPart->Name, MAX_NAMESIZE)))
                        Parts.Add(Kernel.cstring(pPart->Name, MAX_NAMESIZE), (IntPtr)pPart);
                }

                for (var i = 0; i < DumyAmount; i++)
                {
                    var Name = Ini.ReadValue("Dumy", "Dumy" + i) + "\0";

                    var pDumy = (Dumy*)Kernel.calloc(sizeof(Dumy));
                    pDumy->UniqId = i;

                    Name.ToPointer(pStr);
                    Kernel.memcpy(pDumy->Name, pStr, Math.Min(MAX_NAMESIZE - 1, Kernel.strlen(pStr)));

                    if (!Dumies.ContainsKey(pDumy->UniqId))
                        Dumies.Add(pDumy->UniqId, (IntPtr)pDumy);
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified ROPT file (in binary format).
        /// </summary>
        public void SaveToDat(String Path)
        {
            using (var Stream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                lock (Parts)
                lock (Dumies)
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Pointers = new IntPtr[0];

                    Header* pHeader = stackalloc Header[1];
                    pHeader->Identifier = ROPT_IDENTIFIER;
                    pHeader->PartAmount = Parts.Count;
                    pHeader->DumyAmount = Dumies.Count;

                    Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                    Stream.Write(Buffer, 0, sizeof(Header));

                    Pointers = new IntPtr[Parts.Count];
                    Parts.Values.CopyTo(Pointers, 0);

                    for (var i = 0; i < Pointers.Length; i++)
                    {
                        var pPart = (Part*)Pointers[i];
                        Kernel.memcpy(Buffer, pPart, sizeof(Part));
                        Stream.Write(Buffer, 0, sizeof(Part));
                    }

                    Pointers = new IntPtr[Dumies.Count];
                    Dumies.Values.CopyTo(Pointers, 0);

                    for (var i = 0; i < Pointers.Length; i++)
                    {
                        var pDumy = (Dumy*)Pointers[i];
                        Kernel.memcpy(Buffer, pDumy, sizeof(Dumy));
                        Stream.Write(Buffer, 0, sizeof(Dumy));
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified ROPT file (in plain format).
        /// </summary>
        public void SaveToTxt(String Path)
        {
            using (var Stream = new StreamWriter(Path, false, Encoding.GetEncoding("Windows-1252")))
            {
                lock (Parts)
                lock (Dumies)
                {
                    var Pointers = new IntPtr[0];

                    Pointers = new IntPtr[Parts.Count];
                    Parts.Values.CopyTo(Pointers, 0);

                    Stream.WriteLine("[Config]");
                    Stream.WriteLine("Count={0}", Parts.Count);
                    for (var i = 0; i < Pointers.Length; i++)
                    {
                        var pPart = (Part*)Pointers[i];

                        Stream.WriteLine("Part{0}={1}", i, Kernel.cstring(pPart->Name, MAX_NAMESIZE));
                        Stream.WriteLine("MeshIni{0}={1}", i, Kernel.cstring(pPart->MeshIni, MAX_PATHSIZE));
                        Stream.WriteLine("MotionIni{0}={1}", i, Kernel.cstring(pPart->MotionIni, MAX_PATHSIZE));
                    }
                    Stream.WriteLine();

                    Pointers = new IntPtr[Dumies.Count];
                    Dumies.Values.CopyTo(Pointers, 0);

                    Stream.WriteLine("[Dumy]");
                    Stream.WriteLine("Count={0}¡¡¡¡¡¡", Dumies.Count);
                    for (var i = 0; i < Pointers.Length; i++)
                    {
                        var pDumy = (Dumy*)Pointers[i];

                        Stream.WriteLine("Dumy{0}={1}", i, Kernel.cstring(pDumy->Name, MAX_NAMESIZE));
                    }
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                             ropt.cs *
// * ************************************************************