// * ************************************************************
// * * START:                                          mesh.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * MESH class for the library.
// * mesh.cs
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
    /// DBC / MESH
    /// Files: Armet, Armor, Head, Misc, Mount, Weapon
    /// </summary>
    public unsafe class MESH
    {
        public const Int32 MAX_NAMESIZE = 0x20;

        private const Int32 MESH_IDENTIFIER = 0x4853454D;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public Int32 Identifier;
            public Int32 Amount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Entry
        {
            public Int32 UniqId;
            public Int32 Amount;
            public fixed Byte Parts[1];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Part
        {
            public Int32 Mesh;
            public Int32 Texture;
            public Int32 MixTex;
            public Byte MixOpt;
            public Byte Asb;
            public Byte Adb;
            public Byte Material;
        };

        private Dictionary<Int32, IntPtr> Entries = null;

        /// <summary>
        /// Create a new MESH instance to handle the TQ's MESH file.
        /// </summary>
        public MESH()
        {
            this.Entries = new Dictionary<Int32, IntPtr>();
        }

        ~MESH()
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
        /// Load the specified MESH file (in binary format) into the dictionary.
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

                    if (pHeader->Identifier != MESH_IDENTIFIER)
                        throw new Exception("Invalid MESH Header in file: " + Path);

                    for (var i = 0; i < pHeader->Amount; i++)
                    {
                        Stream.Read(Buffer, 0, sizeof(Int32) * 2);

                        var UniqId = 0;
                        var Amount = 0;
                        fixed (Byte* pBuffer = Buffer)
                        {
                            UniqId = *((Int32*)pBuffer);
                            Amount = *((Int32*)pBuffer + 1);
                        }

                        var Length = (sizeof(Entry)) + (Amount * sizeof(Part));

                        var pEntry = (Entry*)Kernel.malloc(Length);
                        Stream.Read(Buffer, 0, Length - sizeof(Entry));
                        Kernel.memcpy(pEntry->Parts, Buffer, Length - sizeof(Entry));

                        pEntry->UniqId = UniqId;
                        pEntry->Amount = Amount;

                        if (!Entries.ContainsKey(pEntry->UniqId))
                            Entries.Add(pEntry->UniqId, (IntPtr)pEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified MESH file (in plain format) into the dictionary.
        /// TODO: The function is really slow... A good optimization is needed!
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Entries)
            {
                var Ini = new Ini(Path);
                using (var Stream = new StreamReader(Path, Encoding.GetEncoding("Windows-1252")))
                {
                    String Line = null;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.StartsWith("["))
                        {
                            var UniqId = Int32.Parse(Line.Substring(1, Line.IndexOf("]") - 1));
                            var Amount = Ini.ReadInt32(UniqId.ToString(), "Part");

                            var Length = sizeof(Entry) + Amount * sizeof(Part);
                            var pEntry = (Entry*)Kernel.malloc(Length);

                            pEntry->UniqId = UniqId;
                            pEntry->Amount = Amount;

                            for (var i = 0; i < Amount; i++)
                            {
                                ((Part*)pEntry->Parts)[i].Mesh = Ini.ReadInt32(UniqId.ToString(), "Mesh" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Texture = Ini.ReadInt32(UniqId.ToString(), "Texture" + i.ToString());
                                ((Part*)pEntry->Parts)[i].MixTex = Ini.ReadInt32(UniqId.ToString(), "MixTex" + i.ToString());
                                ((Part*)pEntry->Parts)[i].MixOpt = Ini.ReadUInt8(UniqId.ToString(), "MixOpt" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Asb = Ini.ReadUInt8(UniqId.ToString(), "Asb" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Adb = Ini.ReadUInt8(UniqId.ToString(), "Adb" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Material = Ini.ReadUInt8(UniqId.ToString(), "Material" + i.ToString());
                            }

                            if (!Entries.ContainsKey(pEntry->UniqId))
                                Entries.Add(pEntry->UniqId, (IntPtr)pEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified MESH file (in binary format).
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
                pHeader->Identifier = MESH_IDENTIFIER;
                pHeader->Amount = Pointers.Length;

                Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                Stream.Write(Buffer, 0, sizeof(Header));

                for (var i = 0; i < Pointers.Length; i++)
                {
                    var Length = sizeof(Entry) + (((Entry*)Pointers[i])->Amount * sizeof(Part));
                    Kernel.memcpy(Buffer, (Entry*)Pointers[i], Length);
                    Stream.Write(Buffer, 0, Length - 1);
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified MESH file (in plain format).
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

                    Stream.WriteLine("[{0}]", pEntry->UniqId);
                    Stream.WriteLine("Part={0}", pEntry->Amount);
                    for (var x = 0; x < pEntry->Amount; x++)
                    {
                        Stream.WriteLine("Mesh{0}={1}", x, ((Part*)pEntry->Parts)[x].Mesh);
                        Stream.WriteLine("Texture{0}={1}", x, ((Part*)pEntry->Parts)[x].Texture);
                        Stream.WriteLine("MixTex{0}={1}", x, ((Part*)pEntry->Parts)[x].MixTex);
                        Stream.WriteLine("MixOpt{0}={1}", x, ((Part*)pEntry->Parts)[x].MixOpt);
                        Stream.WriteLine("Asb{0}={1}", x, ((Part*)pEntry->Parts)[x].Asb);
                        Stream.WriteLine("Adb{0}={1}", x, ((Part*)pEntry->Parts)[x].Adb);
                        if (((Part*)pEntry->Parts)[x].Material == 0)
                            Stream.WriteLine("Material{0}={1}", x, "default");
                        else
                            Stream.WriteLine("Material{0}={1}", x, ((Part*)pEntry->Parts)[x].Material);
                    }
                    Stream.WriteLine();
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                             mesh.cs *
// * ************************************************************