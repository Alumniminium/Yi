// * ************************************************************
// * * START:                                          effe.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * EFFE class for the library.
// * effe.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (February 23th, 2012)
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
    /// DBC / EFFE
    /// Files: 3DEffect
    /// </summary>
    public unsafe class EFFE
    {
        public const Int32 MAX_NAMESIZE = 0x20;

        private const Int32 EFFE_IDENTIFIER = 0x45464645;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public Int32 Identifier;
            public Int32 Amount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Entry
        {
            public fixed Byte Name[MAX_NAMESIZE];
            public Int16 Amount;
            public Int32 Delay;
            public Int32 LoopTime;
            public Int32 FrameInterval;
            public Int32 LoopInterval;
            public Int32 OffsetX;
            public Int32 OffsetY;
            public Int32 OffsetZ;
            public Byte Unknown1; //ZeroFill?
            public Byte ColorEnable;
            public Byte Level;
            public Byte Unknown2; //ZeroFill?
            public fixed Byte Parts[1];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Part
        {
            public Int32 EffectId;
            public Int32 TextureId;
            public Int32 Unknown1; //ZeroFill?
            public Byte Asb;
            public Byte Adb;
            public Int16 Unknown2; //ZeroFill?
        };

        private Dictionary<String, IntPtr> Entries = null;

        /// <summary>
        /// Create a new EFFE instance to handle the TQ's EFFE file.
        /// </summary>
        public EFFE()
        {
            this.Entries = new Dictionary<String, IntPtr>();
        }

        ~EFFE()
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
        /// Load the specified EFFE file (in binary format) into the dictionary.
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

                    if (pHeader->Identifier != EFFE_IDENTIFIER)
                    {
                        Kernel.free(pHeader);
                        throw new Exception("Invalid EFFE Header in file: " + Path);
                    }

                    for (var i = 0; i < pHeader->Amount; i++)
                    {
                        Stream.Seek(MAX_NAMESIZE, SeekOrigin.Current);
                        Stream.Read(Buffer, 0, sizeof(Int16));

                        Int16 Amount = 0;
                        fixed (Byte* pBuffer = Buffer)
                            Amount = *((Int16*)pBuffer);

                        Stream.Seek(-1 * (MAX_NAMESIZE + sizeof(Int16)), SeekOrigin.Current);

                        var Length = (sizeof(Entry)) + (Amount * sizeof(Part));

                        var pEntry = (Entry*)Kernel.calloc(Length);
                        Stream.Read(Buffer, 0, Length - 1);
                        Kernel.memcpy(pEntry, Buffer, Length - 1);

                        if (!Entries.ContainsKey(Kernel.cstring(pEntry->Name, MAX_NAMESIZE)))
                            Entries.Add(Kernel.cstring(pEntry->Name, MAX_NAMESIZE), (IntPtr)pEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified EFFE file (in plain format) into the dictionary.
        /// TODO: The function is really slow... A good optimization is needed!
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Entries)
            {
                var Encoding = Encoding.GetEncoding("Windows-1252");
                var Ini = new Ini(Path);

                using (var Stream = new StreamReader(Path, Encoding))
                {
                    String Line = null;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.StartsWith("["))
                        {
                            var Name = Line.Substring(1, Line.IndexOf("]") - 1);
                            var Amount = Ini.ReadInt16(Name, "Amount");

                            var Length = sizeof(Entry) + Amount * sizeof(Part);
                            var pEntry = (Entry*)Kernel.calloc(Length);

                            Kernel.memcpy(pEntry->Name, Encoding.GetBytes(Name), Math.Min(Name.Length, MAX_NAMESIZE - 1));
                            pEntry->Amount = Amount;

                            pEntry->Delay = Ini.ReadInt32(Name, "Delay");
                            pEntry->LoopTime = Ini.ReadInt32(Name, "LoopTime");
                            pEntry->FrameInterval = Ini.ReadInt32(Name, "FrameInterval");
                            pEntry->LoopInterval = Ini.ReadInt32(Name, "LoopInterval");
                            pEntry->OffsetX = Ini.ReadInt32(Name, "OffsetX");
                            pEntry->OffsetY = Ini.ReadInt32(Name, "OffsetY");
                            pEntry->OffsetZ = Ini.ReadInt32(Name, "OffsetZ");
                            pEntry->ColorEnable = Ini.ReadUInt8(Name, "ColorEnable");
                            pEntry->Level = Ini.ReadUInt8(Name, "Lev");

                            for (var i = 0; i < Amount; i++)
                            {
                                ((Part*)pEntry->Parts)[i].EffectId = Ini.ReadInt32(Name, "EffectId" + i.ToString());
                                ((Part*)pEntry->Parts)[i].TextureId = Ini.ReadInt32(Name, "TextureId" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Asb = Ini.ReadUInt8(Name, "Asb" + i.ToString());
                                ((Part*)pEntry->Parts)[i].Adb = Ini.ReadUInt8(Name, "Adb" + i.ToString());
                            }

                            if (!Entries.ContainsKey(Kernel.cstring(pEntry->Name, MAX_NAMESIZE)))
                                Entries.Add(Kernel.cstring(pEntry->Name, MAX_NAMESIZE), (IntPtr)pEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified EFFE file (in binary format).
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
                pHeader->Identifier = EFFE_IDENTIFIER;
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
        /// Save all the dictionary to the specified EFFE file (in plain format).
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

                    Stream.WriteLine("[{0}]", Kernel.cstring(pEntry->Name, MAX_NAMESIZE));
                    Stream.WriteLine("Amount={0}", pEntry->Amount);
                    for (var x = 0; x < pEntry->Amount; x++)
                    {
                        Stream.WriteLine("EffectId{0}={1}", x, ((Part*)pEntry->Parts)[x].EffectId);
                        Stream.WriteLine("TextureId{0}={1}", x, ((Part*)pEntry->Parts)[x].TextureId);
                        Stream.WriteLine("Asb{0}={1}", x, ((Part*)pEntry->Parts)[x].Asb);
                        Stream.WriteLine("Adb{0}={1}", x, ((Part*)pEntry->Parts)[x].Adb);
                    }
                    Stream.WriteLine("Delay={0}", pEntry->Delay);
                    Stream.WriteLine("LoopTime={0}", pEntry->LoopTime);
                    Stream.WriteLine("FrameInterval={0}", pEntry->FrameInterval);
                    Stream.WriteLine("LoopInterval={0}", pEntry->LoopInterval);
                    Stream.WriteLine("OffsetX={0}", pEntry->OffsetX);
                    Stream.WriteLine("OffsetY={0}", pEntry->OffsetY);
                    Stream.WriteLine("OffsetZ={0}", pEntry->OffsetZ);
                    Stream.WriteLine("ColorEnable={0}", pEntry->ColorEnable);
                    Stream.WriteLine("Lev={0}", pEntry->Level);
                    Stream.WriteLine();
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                             effe.cs *
// * ************************************************************