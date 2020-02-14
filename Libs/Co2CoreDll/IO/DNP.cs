// * ************************************************************
// * * START:                                            dnp.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * TQ Digital DawnPack class for the library.
// * wdf.cs
// * 
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (June 16th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************
// *                      SPECIAL THANKS
// * ************************************************************
// * Sparkie (unknownone @ e*pvp)
// * 
// * ************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//Note: The TQ Digital DawnPack format is derived from the Windsoul Data File format. The hash function
//      used is the same and the overall format is really similar. 
//      DawnPack is known for having two version. The version 1000 used by older games and the version
//      1001. The version 1001 adds a security layout by XORing some fields.

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// DawnPack (TQ Digital)
    /// </summary>
    public unsafe class DNP
    {
        public const String DNP_IDENTIFIER = "DawnPack.TqDigital";
        public const Int32 MAX_IDENTIFIERSIZE = 0x20;

        public const Int32 MIN_VERSION = 1000;
        public const Int32 MAX_VERSION = 1001;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public fixed Byte Identifier[MAX_IDENTIFIERSIZE];
            public Int32 Version;
            public Int32 Number;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public UInt32 UID;
            public UInt32 Size;
            public UInt32 Offset; //Address of the first byte of data
        };

        private Header* pHeader = null;
        private Dictionary<UInt32, IntPtr> Entries = null;

        private String Filename = null;

        public String GetFilename() { return Filename; }
        public Int32 GetAmount() { return pHeader->Number; }

        /// <summary>
        /// Create a new DNP handle.
        /// </summary>
        public DNP()
        {
            this.pHeader = (Header*)Kernel.calloc(sizeof(Header));
            this.Entries = new Dictionary<UInt32, IntPtr>();
        }

        ~DNP()
        {
            Close();
            if (pHeader != null)
                Kernel.free(pHeader);
        }

        /// <summary>
        /// Open the specified DNP package.
        /// </summary>
        public void Open(String Source)
        {
            Close();

            lock (Entries)
            {
                Filename = Source;
                using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                    Stream.Read(Buffer, 0, sizeof(Header));
                    Kernel.memcpy(pHeader, Buffer, sizeof(Header));

                    if (Kernel.cstring(pHeader->Identifier, MAX_IDENTIFIERSIZE) != DNP_IDENTIFIER)
                        throw new Exception("Invalid DNP Header in file: " + Filename);

                    if (pHeader->Version < MIN_VERSION || pHeader->Version > MAX_VERSION)
                        throw new Exception("Unsupported DNP version for file: " + Filename);

                    for (var i = 0; i < pHeader->Number; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                        Stream.Read(Buffer, 0, sizeof(Entry));
                        Kernel.memcpy(pEntry, Buffer, sizeof(Entry));

                        //Version 1001 XOR entry fields
                        if (pHeader->Version == 1001)
                        {
                            pEntry->UID ^= 0x95279527;
                            pEntry->Size ^= 0x96120059;
                            pEntry->Offset ^= 0x99589958;
                        }

                        if (Entries.ContainsKey(pEntry->UID))
                            throw new Exception("Doublon of " + pEntry->UID + " in the file: " + Filename);

                        Entries.Add(pEntry->UID, (IntPtr)pEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Close the file, reset the dictionary and free all the used memory.
        /// </summary>
        public void Close()
        {
            Kernel.memset(pHeader, 0x00, sizeof(Header));
            if (Entries != null)
            {
                lock (Entries)
                {
                    foreach (var pEntry in Entries.Values)
                        Kernel.free((void*)pEntry);
                    Entries.Clear();
                }
            }
        }

        /// <summary>
        /// Check if an entry is linked by the specified unique ID.
        /// </summary>
        public Boolean ContainsEntry(UInt32 ID)
        {
            lock (Entries)
            {
                if (Entries.ContainsKey(ID))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get the information of the entry linked by the specified unique ID.
        /// Return false if the entry does not exist.
        /// </summary>
        public Boolean GetEntryInfo(UInt32 ID, ref Entry Entry)
        {
            Entry = new Entry();
            lock (Entries)
            {
                if (Entries.ContainsKey(ID))
                {
                    fixed (Entry* pEntry = &Entry)
                        Kernel.memcpy(pEntry, (Entry*)Entries[ID], sizeof(Entry));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the data of the entry linked by the specified unique ID.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public Boolean GetEntryData(UInt32 ID, out Byte[] Data)
        {
            Data = null;
            var Length = 0;

            Entry* pEntry = null;
            lock (Entries)
            {
                if (!Entries.ContainsKey(ID))
                    return false;
                pEntry = (Entry*)Entries[ID];
            }

            using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream.Seek(pEntry->Offset, SeekOrigin.Begin);

                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                Data = new Byte[(Int32)pEntry->Size];
                Length = (Int32)pEntry->Size;

                var Count = (Int32)(pEntry->Size / Buffer.Length);
                var Rest = (Int32)(pEntry->Size % Buffer.Length);

                var Pos = 0;
                for (var i = 0; i < Count; i++)
                {
                    Stream.Read(Buffer, 0, Buffer.Length);
                    Array.Copy(Buffer, 0, Data, Pos, Buffer.Length);
                    Pos += Buffer.Length;
                }

                Stream.Read(Buffer, 0, Rest);
                Array.Copy(Buffer, 0, Data, Pos, Rest);
            }
            return true;
        }

        /// <summary>
        /// Get the data of the entry linked by the specified unique ID.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public Boolean GetEntryData(UInt32 ID, Byte** pData, Int32* pLength)
        {
            *pData = null;

            Entry* pEntry = null;
            lock (Entries)
            {
                if (!Entries.ContainsKey(ID))
                    return false;
                pEntry = (Entry*)Entries[ID];
            }

            using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream.Seek(pEntry->Offset, SeekOrigin.Begin);

                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                *pData = (Byte*)Kernel.malloc((Int32)pEntry->Size);
                *pLength = (Int32)pEntry->Size;

                var Count = (Int32)(pEntry->Size / Buffer.Length);
                var Rest = (Int32)(pEntry->Size % Buffer.Length);

                var Pos = 0;
                for (var i = 0; i < Count; i++)
                {
                    Stream.Read(Buffer, 0, Buffer.Length);
                    Kernel.memcpy((*pData) + Pos, Buffer, Buffer.Length);
                    Pos += Buffer.Length;
                }

                Stream.Read(Buffer, 0, Rest);
                Kernel.memcpy((*pData) + Pos, Buffer, Rest);
            }
            return true;
        }

        /// <summary>
        /// Pack the folder pointed by the path (source) in a package pointed by the other path (destination).
        /// A list containing all the unique IDs and the original value will be produced.
        /// </summary>
        public static void Pack(String Source, String Destination, Int32 Version)
        {
            if (Version < MIN_VERSION || Version > MAX_VERSION)
                throw new Exception("Unsupported DNP version!");

            if (!Destination.EndsWith(".dnp"))
                Destination += ".dnp";

            var DI = new DirectoryInfo(Source);
            var Files = DI.GetFiles("*.*", SearchOption.AllDirectories);

            //Really safe implementation...
            var MaxTotalDataSize = (UInt32)(UInt32.MaxValue - sizeof(Header) - (Files.Length * sizeof(Entry)));
            UInt32 TotalDataSize = 0;

            var Offsets = new UInt32[Files.Length];
            for (var i = 0; i < Files.Length; i++)
            {
                if (Files[i].Length > UInt32.MaxValue)
                    throw new Exception(Files[i].FullName + " is bigger than 4 Gio. It is not supported by the format.");

                if (MaxTotalDataSize - TotalDataSize < Files[i].Length)
                    throw new Exception("The folder can't be packed because it is bigger than the 4 Gio that are possible.");

                Offsets[i] = (UInt32)(sizeof(Header) + (Files.Length * sizeof(Entry)) + TotalDataSize);
                TotalDataSize += (UInt32)Files[i].Length;
            }
            //End.

            Header* pHeader = stackalloc Header[1];
            DNP_IDENTIFIER.ToPointer(pHeader->Identifier);
            pHeader->Version = Version;
            pHeader->Number = Files.Length;

            var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
            using (var FStream = new FileStream(Destination, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Console.Write("Writing header... ");
                Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                FStream.Write(Buffer, 0, sizeof(Header));
                Console.WriteLine("Ok!");

                Console.Write("Writing entries... ");
                using (var Writer = new StreamWriter(Destination.Replace(".dnp", ".lst"), false, Encoding.Default))
                {
                    Entry* pEntry = stackalloc Entry[1];
                    for (var i = 0; i < pHeader->Number; i++)
                    {
                        Console.Write("\rWriting entries... {0}%", i * 100 / pHeader->Number);

                        var RelativePath = Files[i].FullName.Replace(DI.Parent.FullName + "\\", "");

                        var UID = String2ID(RelativePath);
                        Writer.WriteLine("{0}={1}", UID, RelativePath);

                        pEntry->UID = UID;
                        pEntry->Size = (UInt32)Files[i].Length;
                        pEntry->Offset = Offsets[i];

                        Kernel.memcpy(Buffer, pEntry, sizeof(Entry));
                        FStream.Write(Buffer, 0, sizeof(Entry));
                    }
                }
                Console.WriteLine("\b\b\bOk!");

                Console.Write("Writing data... ");
                for (var i = 0; i < pHeader->Number; i++)
                {
                    Console.Write("\rWriting data... {0}%", i * 100 / pHeader->Number);

                    using (var Reader = new FileStream(Files[i].FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var Length = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Length > 0)
                        {
                            FStream.Write(Buffer, 0, Length);
                            Length = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                Console.WriteLine("\b\b\bOk!");
            }
        }

        /// <summary>
        /// Convert the specified string to its unique ID.
        /// It have to be used to retreive handle entries.
        /// </summary>
        public static UInt32 String2ID(String Str)
        {
            //x86 - 32 bits - Registers
            UInt32 eax, ebx, ecx, edx, edi, esi;
            UInt64 num = 0;

            UInt32 v;
            Int32 i;
            UInt32* m = stackalloc UInt32[0x46];
            Kernel.memset(m, 0, sizeof(UInt32) * 0x46);
            Byte* buffer = stackalloc Byte[0x100];
            Kernel.memset(buffer, 0, 0x100);

            Str = Str.ToLowerInvariant();
            Str = Str.Replace('/', '\\');

            for (i = 0; i < Str.Length; i++)
                buffer[i] = (Byte)Str[i];

            var Length = (Str.Length % 4 == 0 ? Str.Length / 4 : Str.Length / 4 + 1);
            for (i = 0; i < Length; i++)
                m[i] = *(((UInt32*)buffer) + i);
            m[i++] = 0x9BE74448;
            m[i++] = 0x66F42C48;

            v = 0xF4FA8928;

            edi = 0x7758B42B;
            esi = 0x37A8470E;

            for (ecx = 0; ecx < i; ecx++)
            {
                ebx = 0x267B0B11;
                v = (v << 1) | (v >> 0x1F);
                ebx ^= v;
                eax = m[ecx];
                esi ^= eax;
                edi ^= eax;
                edx = ebx;
                edx += edi;
                edx |= 0x02040801;
                edx &= 0xBFEF7FDF;
                num = edx;
                num *= esi;
                eax = (UInt32)num;
                edx = (UInt32)(num >> 0x20);
                if (edx != 0)
                    eax++;
                num = eax;
                num += edx;
                eax = (UInt32)num;
                if (((UInt32)(num >> 0x20)) != 0)
                    eax++;
                edx = ebx;
                edx += esi;
                edx |= 0x00804021;
                edx &= 0x7DFEFBFF;
                esi = eax;
                num = edi;
                num *= edx;
                eax = (UInt32)num;
                edx = (UInt32)(num >> 0x20);
                num = edx;
                num += edx;
                edx = (UInt32)num;
                if (((UInt32)(num >> 0x20)) != 0)
                    eax++;
                num = eax;
                num += edx;
                eax = (UInt32)num;
                if (((UInt32)(num >> 0x20)) != 0)
                    eax += 2;
                edi = eax;
            }
            esi ^= edi;
            v = esi;

            return v;
        }
    }
}

// * ************************************************************
// * * END:                                              dnp.cs *
// * ************************************************************