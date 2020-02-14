// * ************************************************************
// * * START:                                            wdf.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Windsoul Data File class for the library.
// * wdf.cs
// * 
// * --
// * 
// *  "               ..;;;;,                     ;;;,    
// *  '           ..;;;"` ;;:           ,;;;;;: ,;;;:.,;..          _/
// *  `     ,;..,;;"`    :;;'            :;;"`,;;;;;;"":;;         _/ 
// *        ;;;"  `:;;. ;;'           ..;;:  .;;,.;;:',;;"    _/_/_/_/_/
// *       .;;`   ,;;" .;"          ,;;;;;;" :;`',;",;;"         _/
// *      ,;;,:.,;;;  ,;:          :" ,;:` , `:.;;;;;'`         _/   
// *      ;;"'':;;:. .;; .          ,;;;,;:;;,;;;, ,;             _/
// *     :;;..;;;;;; :;' :.        :;;;"` `:;;;;;,;,.;.          _/
// *   .;;":;;`  '"";;:  ';;       '""   .;;`.;";:;;;;` ;,  _/_/_/_/_/
// *  ;;;" `'       "::. ,;;:          .;"`  ::. '   .,;;;     _/ 
// *  ""             ';;;;;;;"        ""     ';;;;;;;;;;`     _/
// *  
// *                         Windsoul++
// * 
// *                 by ÔÆ·ç (Cloud Wu)  1999-2001
// *  
// * 		http://member.netease.com/~cloudwu 
// * 		mailto:cloudwu@263.net
// *  
// * 		ÇëÔÄ¶Á readme.txt ÖÐµÄ°æÈ¨ÐÅÏ¢
// * 		See readme.txt for copyright information.
// * 
// * 		Description:		·ç»ê++ Êý¾ÝÎÄ¼þ¹ÜÀí
// *  		Original Author:	ÔÆ·ç
// * 		Authors:
// * 		Create Time:		2000/10/16
// * 		Modify Time:		2001/12/26
// * 
// * .:*W*:._.:*I*:._.:*N*:._.:*D*:._.:*S*:._.:*O*:._.:*U*:._.:*L*:._.:*/
// * 
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by Cloud Wu (October 16th, 2000)
// * Copyright (C) 2000-2001 Cloud Wu
// *  
// * Implemented by CptSky (January 12th, 2012)
// * Copyright (C) 2012 CptSky
// *
// * ************************************************************
// *                      SPECIAL THANKS
// * ************************************************************
// * Cloud Wu
// * Sparkie (unknownone @ e*pvp)
// * 
// * ************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//An important note about this implementation is that it is really not optimized! As there is no specification
//on the format, and because it is a complete format, it is hard to optimize the implementation. For example,
//when deleting an entry, it would be better to zero-fill the entry and to declare it as "free", but as it is
//impossible to predict how the client will react with that, a safe implementation as been made. Please keep in
//mind that adding/deleting/updating an entry may result in the corruption of the package and that it can take
//a really long time. Honestly, the way the package is made is really awesome as the structure can be altered
//easily if the implementation is good, but I doubt about the implementation of TQ... Commonly, the package is made
//of Header | Data | Entries, but it can be made of Header | Entries | Data. This implementation work with the files
//as there are seen in the client. It should always produce a valid result for CO2.

//Note: The Windsoul Data File is from the Windsoul++ Game Engine, developped by Cloud Wu. It seems that TQ uses
//      some part of the game engine in their own games. 
//      http://en.pudn.com/downloads76/sourcecode/game/detail281928_en.html

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// Windsould Date File
    /// </summary>
    public unsafe class WDF
    {
        public const UInt32 WDF_ID = 0x57444650;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt32 Id;
            public Int32 Number;
            public UInt32 Offset; //Address of the first entry
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public UInt32 UID;
            public UInt32 Offset; //Address of the first byte of data
            public UInt32 Size;
            public UInt32 Space;
        };

        private Header* pHeader = null;
        private Dictionary<UInt32, IntPtr> Entries = null;

        private String Filename = null;

        public String GetFilename() { return Filename; }
        public Int32 GetAmount() { return pHeader->Number; }

        /// <summary>
        /// Create a new WDF handle.
        /// </summary>
        public WDF()
        {
            this.pHeader = (Header*)Kernel.calloc(sizeof(Header));
            this.Entries = new Dictionary<UInt32, IntPtr>();
        }

        ~WDF()
        {
            Close();
            if (pHeader != null)
                Kernel.free(pHeader);
        }

        /// <summary>
        /// Open the specified WDF package.
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

                    if (pHeader->Id != WDF_ID)
                        throw new Exception("Invalid WDF Header in file: " + Filename);

                    Stream.Seek(pHeader->Offset, SeekOrigin.Begin);
                    for (var i = 0; i < pHeader->Number; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                        Stream.Read(Buffer, 0, sizeof(Entry));
                        Kernel.memcpy(pEntry, Buffer, sizeof(Entry));

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
        /// Add an entry and link it by the specified unique ID.
        /// It is not recommended to use this function on big package for adding
        /// several files. 
        /// 
        /// An interruption during the execution will corrupt the archive as the 
        /// implementation is not completely safe... (Lack of motivation.)
        /// 
        /// Note: The function do not check if the size will overflow the package...
        ///       If the current size of the package plus the size of the file is
        ///       arround 4 GiB, avoid using this function...
        /// </summary>
        public Boolean AddEntry(UInt32 ID, String Source)
        {
            lock (Entries)
            {
                if (Entries.ContainsKey(ID))
                    return false;

                if (!File.Exists(Source))
                    return false;

                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                pEntry->UID = ID;
                pEntry->Offset = pHeader->Offset;
                pEntry->Size = (UInt32)new FileInfo(Source).Length;
                pEntry->Space = 0;

                //Move all the data to make place
                var TmpPath = Path.GetTempFileName();
                using (var Reader = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(TmpPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Reader.Seek(pHeader->Offset, SeekOrigin.Begin);

                        for (var i = 0; i < pHeader->Number; i++)
                        {
                            Reader.Read(Buffer, 0, sizeof(Entry));
                            Writer.Write(Buffer, 0, sizeof(Entry));
                        }
                    }
                }
                using (var Reader = new FileStream(TmpPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Writer.Seek(pHeader->Offset + pEntry->Size, SeekOrigin.Begin);

                        for (var i = 0; i < pHeader->Number; i++)
                        {
                            Reader.Read(Buffer, 0, sizeof(Entry));
                            Writer.Write(Buffer, 0, sizeof(Entry));
                        }
                    }
                }
                File.Delete(TmpPath);

                //Write new data
                using (var Reader = new FileStream(Source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Writer.Seek(pHeader->Offset, SeekOrigin.Begin);

                        var Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Read > 0)
                        {
                            Writer.Write(Buffer, 0, Read);
                            Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Write new offsets
                pHeader->Number++;
                pHeader->Offset += pEntry->Size;
                using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                    Writer.Write(Buffer, 0, sizeof(Header));

                    Writer.Seek(0, SeekOrigin.End);
                    Kernel.memcpy(Buffer, pEntry, sizeof(Entry));
                    Writer.Write(Buffer, 0, sizeof(Entry));
                }

                Entries.Add(ID, (IntPtr)pEntry);
                return true;
            }
        }

        /// <summary>
        /// Delete the entry that is linked by the specified unique ID.
        /// It is not recommended to use this function on big package for removing
        /// small file as the saved size will probably be not signifiant for the
        /// time required.
        /// 
        /// An interruption during the execution will corrupt the archive as the 
        /// implementation is not completely safe... (Lack of motivation.)
        /// </summary>
        public Boolean DelEntry(UInt32 ID)
        {
            Entry* pEntry = null;
            lock (Entries)
            {
                if (!Entries.ContainsKey(ID))
                    return false;
                pEntry = (Entry*)Entries[ID];

                var Info = new FileInfo(Filename);
                var Size = Info.Length;

                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                //Update the header.
                pHeader->Number--;
                pHeader->Offset -= pEntry->Size;
                using (var Stream = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.Read))
                {
                    Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                    Stream.Write(Buffer, 0, sizeof(Header));
                }

                //Move all the data (Remove the entry's data)
                using (var Reader = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Reader.Seek(pEntry->Offset + pEntry->Size, SeekOrigin.Begin);
                        Writer.Seek(pEntry->Offset, SeekOrigin.Begin);

                        var Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Read > 0)
                        {
                            Writer.Write(Buffer, 0, Read);
                            Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Change all the entries offset & truncate the file (Remove the entry's info)
                using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    Writer.Seek(pHeader->Offset, SeekOrigin.Begin);
                    foreach (var Ptr in Entries.Values)
                    {
                        var pTmpEntry = (Entry*)Ptr;
                        if (pTmpEntry->UID == pEntry->UID)
                            continue;

                        if (pTmpEntry->Offset > pEntry->Offset)
                            pTmpEntry->Offset -= pEntry->Size;

                        Kernel.memcpy(Buffer, pTmpEntry, sizeof(Entry));
                        Writer.Write(Buffer, 0, sizeof(Entry));
                    }
                    Writer.SetLength(Size - pEntry->Size - sizeof(Entry));
                }

                //Remove the entry from the dictionnary.
                Entries.Remove(ID);

                Kernel.free(pEntry);
                return true;
            }
        }

        /// <summary>
        /// Update the entry that is linked by the specified unique ID.
        /// It is not recommended to use this function on big package for updating
        /// several files. 
        /// 
        /// An interruption during the execution will corrupt the archive as the 
        /// implementation is not completely safe... (Lack of motivation.)
        /// 
        /// Note: The function do not check if the size will overflow the package...
        ///       If the current size of the package plus the size of the file is
        ///       arround 4 GiB, avoid using this function...
        /// </summary>
        public Boolean UpdateEntry(UInt32 ID, String Source)
        {
            lock (Entries)
            {
                if (!Entries.ContainsKey(ID))
                    return false;

                if (!File.Exists(Source))
                    return false;

                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                var Info = new FileInfo(Source);
                var pEntry = (Entry*)Entries[ID];

                //Move all the data to make place
                var TmpPath = Path.GetTempFileName();
                using (var Reader = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(TmpPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Reader.Seek(pEntry->Offset + pEntry->Size, SeekOrigin.Begin);
                        Writer.Seek(0, SeekOrigin.Begin);

                        var Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Read > 0)
                        {
                            Writer.Write(Buffer, 0, Read);
                            Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                using (var Reader = new FileStream(TmpPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Reader.Seek(0, SeekOrigin.Begin);
                        Writer.Seek(pEntry->Offset + Info.Length, SeekOrigin.Begin);

                        var Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Read > 0)
                        {
                            Writer.Write(Buffer, 0, Read);
                            Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                File.Delete(TmpPath);

                //Write new data
                using (var Reader = new FileStream(Source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        Writer.Seek(pEntry->Offset, SeekOrigin.Begin);

                        var Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (Read > 0)
                        {
                            Writer.Write(Buffer, 0, Read);
                            Read = Reader.Read(Buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Change all the offset & size & truncate if required
                using (var Writer = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    pHeader->Offset = (UInt32)((Int64)pHeader->Offset + (Info.Length - (Int64)pEntry->Size));
                    Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                    Writer.Write(Buffer, 0, sizeof(Header));

                    Writer.Seek(pHeader->Offset, SeekOrigin.Begin);
                    foreach (var Ptr in Entries.Values)
                    {
                        var pTmpEntry = (Entry*)Ptr;
                        if (pTmpEntry->UID == pEntry->UID)
                            continue;

                        if (pTmpEntry->Offset > pEntry->Offset)
                            pTmpEntry->Offset = (UInt32)((Int64)pTmpEntry->Offset + (Info.Length - (Int64)pEntry->Size));

                        Kernel.memcpy(Buffer, pTmpEntry, sizeof(Entry));
                        Writer.Write(Buffer, 0, sizeof(Entry));
                    }

                    var OldSize = pEntry->Size;
                    pEntry->Size = (UInt32)Info.Length;

                    Kernel.memcpy(Buffer, pEntry, sizeof(Entry));
                    Writer.Write(Buffer, 0, sizeof(Entry));

                    if (Info.Length < OldSize)
                        Writer.SetLength(new FileInfo(Filename).Length - (OldSize - Info.Length));
                }

                return true;
            }
        }

        /// <summary>
        /// Pack the folder pointed by the path (source) in a package pointed by the other path (destination).
        /// A list containing all the unique IDs and the original value will be produced.
        /// </summary>
        public static void Pack(String Source, String Destination)
        {
            if (!Destination.EndsWith(".wdf"))
                Destination += ".wdf";

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

                Offsets[i] = (UInt32)(sizeof(Header) + TotalDataSize);
                TotalDataSize += (UInt32)Files[i].Length;
            }
            //End.

            Header* pHeader = stackalloc Header[1];
            pHeader->Id = WDF_ID;
            pHeader->Number = Files.Length;
            pHeader->Offset = (UInt32)(sizeof(Header) + TotalDataSize);

            var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
            using (var FStream = new FileStream(Destination, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Console.Write("Writing header... ");
                Kernel.memcpy(Buffer, pHeader, sizeof(Header));
                FStream.Write(Buffer, 0, sizeof(Header));
                Console.WriteLine("Ok!");

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

                Console.Write("Writing entries... ");
                using (var Writer = new StreamWriter(Destination.Replace(".wdf", ".lst"), false, Encoding.Default))
                {
                    Entry* pEntry = stackalloc Entry[1];
                    for (var i = 0; i < pHeader->Number; i++)
                    {
                        Console.Write("\rWriting entries... {0}%", i * 100 / pHeader->Number);

                        var RelativePath = Files[i].FullName.Replace(DI.Parent.FullName + "\\", "");
                        RelativePath = RelativePath.ToLowerInvariant();
                        RelativePath = RelativePath.Replace('/', '\\');

                        var UID = String2ID(RelativePath);
                        Writer.WriteLine("{0}={1}", UID, RelativePath);

                        pEntry->UID = UID;
                        pEntry->Offset = Offsets[i];
                        pEntry->Size = (UInt32)Files[i].Length;
                        pEntry->Space = 0;

                        Kernel.memcpy(Buffer, pEntry, sizeof(Entry));
                        FStream.Write(Buffer, 0, sizeof(Entry));
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
            Str = Str.Replace('\\', '/');

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
// * * END:                                              wdf.cs *
// * ************************************************************