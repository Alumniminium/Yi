// * ************************************************************
// * * START:                                            tpi.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * TPI class for the library.
// * tpi.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (March 13th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ComponentAce.Compression.Libs.zlib;

namespace CO2_CORE_DLL.IO
{
    public partial class NetDragonDatPkg
    {
        /// <summary>
        /// NetDragon Data Package (Information)
        /// </summary>
        private unsafe class TPI
        {
            public const Int32 MAX_IDENTIFIERSIZE = 0x10;

            public const String TPI_IDENTIFIER = "NetDragonDatPkg";
            public const Int64 TPI_VERSION = 1000;

            public const Int32 TPI_UNKNOWN_1 = 0x01;
            public const Int32 TPI_UNKNOWN_2 = 0x03;
            public const Int32 TPI_UNKNOWN_3 = 0x30;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Header
            {
                public fixed Byte Identifier[MAX_IDENTIFIERSIZE];
                public Int64 Version;
                public Int32 Unknown1; //0x01
                public Int32 Unknown2; //0x03
                public Int32 Unknown3; //0x30
                public UInt32 Number;
                public UInt32 Offset; //Seems to be the last entry offset...
                public Int32 Reserved; //0x00
            };

            public struct Entry
            {
                //public Byte Path_Length
                public String Path;
                public Int16 Unknown1; //0x01
                public UInt32 UncompressedSize;
                public UInt32 CompressedSize;
                //public UInt32 UncompressedSize2;
                //public UInt32 CompressedSize2;
                public UInt32 Offset;
            }

            private Encoding Encoding = Encoding.GetEncoding("Windows-1252");

            private TPD TpdFile = null;
            private Header* pHeader = null;
            private Dictionary<String, Entry> Entries = null;
            private String Filename = null;

            public String GetFilename() { return Filename; }
            public UInt32 GetAmount() { return pHeader->Number; }

            /// <summary>
            /// Create a new TPI handle.
            /// </summary>
            public TPI()
            {
                this.pHeader = (Header*)Kernel.calloc(sizeof(Header));
                this.Entries = new Dictionary<String, Entry>();
            }

            ~TPI()
            {
                Close();
                if (pHeader != null)
                    Kernel.free(pHeader);
                TpdFile = null;
            }

            /// <summary>
            /// Open the specified TPI package.
            /// </summary>
            public void Open(String Source)
            {
                Close();

                lock (Entries)
                {
                    Filename = Source;
                    if (!File.Exists(Source.ToLower().Replace(".tpi", ".tpd")))
                        throw new Exception("The TPI file does not have it's TPD equivalent: " + Filename);
                    TpdFile = new TPD();
                    TpdFile.Open(Source.ToLower().Replace(".tpi", ".tpd"));

                    using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                        Stream.Read(Buffer, 0, sizeof(Header));
                        Kernel.memcpy(pHeader, Buffer, sizeof(Header));

                        if (Kernel.cstring(pHeader->Identifier, MAX_IDENTIFIERSIZE) != TPI_IDENTIFIER)
                            throw new Exception("Invalid TPI Header in file: " + Filename);

                        if (pHeader->Version != TPI_VERSION)
                            throw new Exception("Unsupported TPI version!");

                        using (var Reader = new BinaryReader(Stream, Encoding))
                        {
                            for (var i = 0; i < pHeader->Number; i++)
                            {
                                var Entry = new Entry();

                                Entry.Path = Encoding.GetString(Reader.ReadBytes(Reader.ReadByte()));
                                Entry.Unknown1 = Reader.ReadInt16();
                                Entry.UncompressedSize = Reader.ReadUInt32();
                                Entry.CompressedSize = Reader.ReadUInt32();
                                if (Reader.ReadUInt32() != Entry.CompressedSize)
                                    continue;
                                if (Reader.ReadUInt32() != Entry.UncompressedSize)
                                    continue;
                                Entry.Offset = Reader.ReadUInt32();

                                if (!Entries.ContainsKey(Entry.Path))
                                    Entries.Add(Entry.Path, Entry);
                            }
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
                    lock (Entries) { Entries.Clear(); }
                }
                if (TpdFile != null)
                    TpdFile.Close();
                TpdFile = null;
            }

            /// <summary>
            /// Check if an entry is linked by the specified path.
            /// </summary>
            public Boolean ContainsEntry(String Path)
            {
                if (Path.StartsWith("/") || Path.StartsWith("\\"))
                    return false;

                lock (Entries)
                {
                    if (Entries.ContainsKey(Path.ToLowerInvariant().Replace('\\', '/')))
                        return true;
                }
                return false;
            }

            /// <summary>
            /// Get the data of the entry linked by the specified path.
            /// All the data will be allocated in memory. It may fail.
            /// 
            /// Return false if the entry does not exist.
            /// </summary>
            public Boolean GetEntryData(String Path, out Byte[] Data)
            {
                Data = null;

                if (Path.StartsWith("/") || Path.StartsWith("\\"))
                    return false;

                Entry Entry;
                lock (Entries)
                {
                    if (!Entries.TryGetValue(Path.ToLowerInvariant().Replace('\\', '/'), out Entry))
                        return false;
                }

                Data = new Byte[(Int32)Entry.UncompressedSize];
                using (var Input = new FileStream(TpdFile.GetFilename(), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Tmp = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Pos = 0;

                    Input.Seek(Entry.Offset, SeekOrigin.Begin);

                    {
                        var Stream = new ZStream();
                        Stream.inflateInit();

                        var Result = 0;
                        var Length = 0;
                        do
                        {
                            Stream.avail_in = Input.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                            Stream.next_in = Buffer;
                            Stream.next_in_index = 0;

                            if (Stream.avail_in == 0)
                                break;

                            do
                            {
                                Stream.avail_out = Kernel.MAX_BUFFER_SIZE;
                                Stream.next_out = Tmp;
                                Stream.next_out_index = 0;

                                Result = Stream.inflate(zlibConst.Z_NO_FLUSH);

                                Length = Kernel.MAX_BUFFER_SIZE - Stream.avail_out;
                                Array.Copy(Buffer, 0, Data, Pos, Length);
                                Pos += Length;
                            }
                            while (Stream.avail_out == 0);
                        }
                        while (Result != zlibConst.Z_STREAM_END);

                        Stream.inflateEnd();
                    }
                }
                return true;
            }

            /// <summary>
            /// Get the data of the entry linked by the specified path.
            /// All the data will be allocated in memory. It may fail.
            /// 
            /// Return false if the entry does not exist.
            /// </summary>
            public Boolean GetEntryData(String Path, Byte** pData, Int32* pLength)
            {
                *pData = null;

                if (Path.StartsWith("/") || Path.StartsWith("\\"))
                    return false;

                Entry Entry;
                lock (Entries)
                {
                    if (!Entries.TryGetValue(Path.ToLowerInvariant().Replace('\\', '/'), out Entry))
                        return false;
                }

                *pData = (Byte*)Kernel.malloc((Int32)Entry.UncompressedSize);
                *pLength = (Int32)Entry.UncompressedSize;
                using (var Input = new FileStream(TpdFile.GetFilename(), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Tmp = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Pos = 0;

                    Input.Seek(Entry.Offset, SeekOrigin.Begin);

                    {
                        var Stream = new ZStream();
                        Stream.inflateInit();

                        var Result = 0;
                        var Length = 0;
                        do
                        {
                            Stream.avail_in = Input.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                            Stream.next_in = Buffer;
                            Stream.next_in_index = 0;

                            if (Stream.avail_in == 0)
                                break;

                            do
                            {
                                Stream.avail_out = Kernel.MAX_BUFFER_SIZE;
                                Stream.next_out = Tmp;
                                Stream.next_out_index = 0;

                                Result = Stream.inflate(zlibConst.Z_NO_FLUSH);

                                Length = Kernel.MAX_BUFFER_SIZE - Stream.avail_out;
                                Kernel.memcpy((*pData) + Pos, Tmp, Length);
                                Pos += Length;
                            }
                            while (Stream.avail_out == 0);
                        }
                        while (Result != zlibConst.Z_STREAM_END);

                        Stream.inflateEnd();
                    }
                }
                return true;
            }

            /// <summary>
            /// Extract all files contained in the package in the folder pointed by the destination path.
            /// </summary>
            public void ExtractAll(String Destination)
            {
                Destination = Destination.Replace('/', '\\');
                if (!Destination.EndsWith("\\"))
                    Destination += "\\";

                using (var Input = new FileStream(TpdFile.GetFilename(), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    var Tmp = new Byte[Kernel.MAX_BUFFER_SIZE];

                    foreach (var Entry in Entries.Values)
                    {
                        var DestPath = Destination + Entry.Path.Replace("/", "\\");
                        if (!Directory.Exists(Path.GetDirectoryName(DestPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(DestPath));

                        Input.Seek(Entry.Offset, SeekOrigin.Begin);
                        using (var Output = new FileStream(DestPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        {
                            var Stream = new ZStream();
                            Stream.inflateInit();

                            var Result = 0;
                            var Length = 0;
                            do
                            {
                                Stream.avail_in = Input.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                                Stream.next_in = Buffer;
                                Stream.next_in_index = 0;

                                if (Stream.avail_in == 0)
                                    break;

                                do
                                {
                                    Stream.avail_out = Kernel.MAX_BUFFER_SIZE;
                                    Stream.next_out = Tmp;
                                    Stream.next_out_index = 0;

                                    Result = Stream.inflate(zlibConst.Z_NO_FLUSH);

                                    Length = Kernel.MAX_BUFFER_SIZE - Stream.avail_out;
                                    Output.Write(Tmp, 0, Length);
                                }
                                while (Stream.avail_out == 0);
                            }
                            while (Result != zlibConst.Z_STREAM_END);

                            Stream.inflateEnd();
                        }
                    }
                }
            }

            /// <summary>
            /// Pack the folder pointed by the path (source) in a package pointed by the other path (destination).
            /// </summary>
            public static void Pack(String Source, String Destination)
            {
                if (!Destination.EndsWith(".tpi"))
                    Destination += ".tpi";

                var DI = new DirectoryInfo(Source);
                var Files = DI.GetFiles("*.*", SearchOption.AllDirectories);

                Header* pHeader = stackalloc Header[1];
                TPI_IDENTIFIER.ToPointer(pHeader->Identifier);
                pHeader->Number = (UInt32)Files.Length;
                pHeader->Version = TPI_VERSION;
                pHeader->Unknown1 = TPI_UNKNOWN_1;
                pHeader->Unknown2 = TPI_UNKNOWN_2;
                pHeader->Unknown3 = TPI_UNKNOWN_3;
                pHeader->Reserved = 0x00;

                var Encoding = Encoding.GetEncoding("Windows-1252");
                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                var Tmp = new Byte[Kernel.MAX_BUFFER_SIZE];

                using (var TpiStream = new FileStream(Destination, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var TpdStream = new FileStream(Destination.Replace(".tpi", ".tpd"), FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (var Writer = new BinaryWriter(TpiStream, Encoding))
                    {
                        Console.Write("Writing header... ");
                        Kernel.memcpy(Buffer, pHeader, sizeof(TPI.Header));
                        TpiStream.Write(Buffer, 0, sizeof(TPI.Header));
                        TpdStream.Write(Buffer, 0, sizeof(TPD.Header));
                        Console.WriteLine("Ok!");

                        Console.Write("Writing data... ");
                        var CompressedSizes = new UInt32[Files.Length];
                        var Offsets = new UInt32[Files.Length];
                        var Offset = (UInt32)sizeof(TPD.Header);

                        for (var i = 0; i < pHeader->Number; i++)
                        {
                            Console.Write("\rWriting data... {0}%", i * 100 / pHeader->Number);

                            using (var Input = new FileStream(Files[i].FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                var Stream = new ZStream();
                                Stream.deflateInit(9); //TQ use lvl 3

                                var Param = zlibConst.Z_NO_FLUSH;
                                var Length = 0;
                                do
                                {
                                    Length = Input.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);

                                    Param = Length == 0 ? zlibConst.Z_FINISH : zlibConst.Z_NO_FLUSH;
                                    Stream.avail_in = Length;
                                    Stream.next_in = Buffer;
                                    Stream.next_in_index = 0;
                                    do
                                    {
                                        Stream.avail_out = Kernel.MAX_BUFFER_SIZE;
                                        Stream.next_out = Tmp;
                                        Stream.next_out_index = 0;

                                        var Result = Stream.deflate(Param);

                                        var Len = Kernel.MAX_BUFFER_SIZE - Stream.avail_out;
                                        TpdStream.Write(Tmp, 0, Len);
                                    }
                                    while (Stream.avail_out == 0);
                                }
                                while (Param != zlibConst.Z_FINISH);

                                Stream.deflateEnd();

                                CompressedSizes[i] = (UInt32)Stream.total_out;
                                Offsets[i] = Offset;
                                Offset += CompressedSizes[i];
                            }
                        }
                        Console.WriteLine("\b\b\bOk!");

                        Console.Write("Writing entries... ");
                        UInt32 LastOffset = 0;

                        for (var i = 0; i < pHeader->Number; i++)
                        {
                            Console.Write("\rWriting entries... {0}%", i * 100 / pHeader->Number);

                            var RelativePath = Files[i].FullName.Replace(DI.Parent.FullName + "\\", "");
                            RelativePath = RelativePath.ToLowerInvariant();
                            RelativePath = RelativePath.Replace('\\', '/');

                            LastOffset = (UInt32)Writer.BaseStream.Position;

                            Writer.Write((Byte)RelativePath.Length);
                            Writer.Write(Encoding.GetBytes(RelativePath.ToCharArray(), 0, (Byte)RelativePath.Length));
                            Writer.Write((Int16)TPI_UNKNOWN_1);
                            Writer.Write((UInt32)Files[i].Length);
                            Writer.Write((UInt32)CompressedSizes[i]);
                            Writer.Write((UInt32)CompressedSizes[i]);
                            Writer.Write((UInt32)Files[i].Length);
                            Writer.Write((UInt32)Offsets[i]);
                        }

                        Console.WriteLine("\b\b\bOk!");

                        Console.Write("Updating header... ");
                        TpiStream.Seek(0, SeekOrigin.Begin);
                        pHeader->Offset = LastOffset;

                        Kernel.memcpy(Buffer, pHeader, sizeof(TPI.Header));
                        TpiStream.Write(Buffer, 0, sizeof(TPI.Header));
                        Console.WriteLine("Ok!");
                    }
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                              tpi.cs *
// * ************************************************************