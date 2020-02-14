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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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

namespace ItemCreator.CoreDLL.IO
{
    /// <summary>
    /// Windsould Date File
    /// </summary>
    public unsafe class Wdf
    {
        public const uint WdfId = 0x57444650;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public uint Id;
            public int Number;
            public uint Offset; //Address of the first entry
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public uint UID;
            public uint Offset; //Address of the first byte of data
            public uint Size;
            public uint Space;
        };

        private readonly Header* _pHeader;
        private readonly Dictionary<uint, IntPtr> _entries;

        private string _filename;

        public string GetFilename() => _filename;
        public int GetAmount() => _pHeader->Number;

        /// <summary>
        /// Create a new WDF handle.
        /// </summary>
        public Wdf()
        {
            _pHeader = (Header*)Kernel.calloc(sizeof(Header));
            _entries = new Dictionary<uint, IntPtr>();
        }

        ~Wdf()
        {
            Close();
            if (_pHeader != null)
                Kernel.free(_pHeader);
        }

        /// <summary>
        /// Open the specified WDF package.
        /// </summary>
        public void Open(string source)
        {
            Close();

            lock (_entries)
            {
                _filename = source;
                using (var stream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[Kernel.MAX_BUFFER_SIZE];

                    stream.Read(buffer, 0, sizeof(Header));
                    Kernel.memcpy(_pHeader, buffer, sizeof(Header));

                    if (_pHeader->Id != WdfId)
                        throw new Exception("Invalid WDF Header in file: " + _filename);

                    stream.Seek(_pHeader->Offset, SeekOrigin.Begin);
                    for (var i = 0; i < _pHeader->Number; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                        stream.Read(buffer, 0, sizeof(Entry));
                        Kernel.memcpy(pEntry, buffer, sizeof(Entry));

                        if (_entries.ContainsKey(pEntry->UID))
                            throw new Exception("Doublon of " + pEntry->UID + " in the file: " + _filename);

                        _entries.Add(pEntry->UID, (IntPtr)pEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Close the file, reset the dictionary and free all the used memory.
        /// </summary>
        public void Close()
        {
            Kernel.memset(_pHeader, 0x00, sizeof(Header));
            if (_entries == null) return;
            lock (_entries)
            {
                foreach (var pEntry in _entries.Values)
                    Kernel.free((void*)pEntry);
                _entries.Clear();
            }
        }

        /// <summary>
        /// Check if an entry is linked by the specified unique ID.
        /// </summary>
        public bool ContainsEntry(uint id)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey(id))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Get the data of the entry linked by the specified unique ID.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public bool GetEntryData(uint id, out byte[] data)
        {
            data = null;

            Entry* pEntry;
            lock (_entries)
            {
                if (!_entries.ContainsKey(id))
                    return false;
                pEntry = (Entry*)_entries[id];
            }

            using (var stream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(pEntry->Offset, SeekOrigin.Begin);

                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                data = new byte[(int)pEntry->Size];

                var count = (int)(pEntry->Size / buffer.Length);
                var rest = (int)(pEntry->Size % buffer.Length);

                var pos = 0;
                for (var i = 0; i < count; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    Array.Copy(buffer, 0, data, pos, buffer.Length);
                    pos += buffer.Length;
                }

                stream.Read(buffer, 0, rest);
                Array.Copy(buffer, 0, data, pos, rest);
            }
            return true;
        }

        /// <summary>
        /// Get the data of the entry linked by the specified unique ID.
        /// All the data will be allocated in memory. It may fail.
        /// 
        /// Return false if the entry does not exist.
        /// </summary>
        public bool GetEntryData(uint id, byte** pData, int* pLength)
        {
            *pData = null;

            Entry* pEntry;
            lock (_entries)
            {
                if (!_entries.ContainsKey(id))
                    return false;
                pEntry = (Entry*)_entries[id];
            }

            using (var stream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(pEntry->Offset, SeekOrigin.Begin);

                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                *pData = (byte*)Kernel.malloc((int)pEntry->Size);
                *pLength = (int)pEntry->Size;

                var count = (int)(pEntry->Size / buffer.Length);
                var rest = (int)(pEntry->Size % buffer.Length);

                var pos = 0;
                for (var i = 0; i < count; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    Kernel.memcpy(*pData + pos, buffer, buffer.Length);
                    pos += buffer.Length;
                }

                stream.Read(buffer, 0, rest);
                Kernel.memcpy(*pData + pos, buffer, rest);
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
        public bool AddEntry(uint id, string source)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey(id))
                    return false;

                if (!File.Exists(source))
                    return false;

                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];

                var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                pEntry->UID = id;
                pEntry->Offset = _pHeader->Offset;
                pEntry->Size = (uint)new FileInfo(source).Length;
                pEntry->Space = 0;

                //Move all the data to make place
                var tmpPath = Path.GetTempFileName();
                using (var reader = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(tmpPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        reader.Seek(_pHeader->Offset, SeekOrigin.Begin);

                        for (var i = 0; i < _pHeader->Number; i++)
                        {
                            reader.Read(buffer, 0, sizeof(Entry));
                            writer.Write(buffer, 0, sizeof(Entry));
                        }
                    }
                }
                using (var reader = new FileStream(tmpPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        writer.Seek(_pHeader->Offset + pEntry->Size, SeekOrigin.Begin);

                        for (var i = 0; i < _pHeader->Number; i++)
                        {
                            reader.Read(buffer, 0, sizeof(Entry));
                            writer.Write(buffer, 0, sizeof(Entry));
                        }
                    }
                }
                File.Delete(tmpPath);

                //Write new data
                using (var reader = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        writer.Seek(_pHeader->Offset, SeekOrigin.Begin);

                        var read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (read > 0)
                        {
                            writer.Write(buffer, 0, read);
                            read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Write new offsets
                _pHeader->Number++;
                _pHeader->Offset += pEntry->Size;
                using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    Kernel.memcpy(buffer, _pHeader, sizeof(Header));
                    writer.Write(buffer, 0, sizeof(Header));

                    writer.Seek(0, SeekOrigin.End);
                    Kernel.memcpy(buffer, pEntry, sizeof(Entry));
                    writer.Write(buffer, 0, sizeof(Entry));
                }

                _entries.Add(id, (IntPtr)pEntry);
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
        public bool DelEntry(uint id)
        {
            lock (_entries)
            {
                if (!_entries.ContainsKey(id))
                    return false;
                var pEntry = (Entry*)_entries[id];

                var info = new FileInfo(_filename);
                var size = info.Length;

                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];

                //Update the header.
                _pHeader->Number--;
                _pHeader->Offset -= pEntry->Size;
                using (var stream = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.Read))
                {
                    Kernel.memcpy(buffer, _pHeader, sizeof(Header));
                    stream.Write(buffer, 0, sizeof(Header));
                }

                //Move all the data (Remove the entry's data)
                using (var reader = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        reader.Seek(pEntry->Offset + pEntry->Size, SeekOrigin.Begin);
                        writer.Seek(pEntry->Offset, SeekOrigin.Begin);

                        var read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (read > 0)
                        {
                            writer.Write(buffer, 0, read);
                            read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Change all the entries offset & truncate the file (Remove the entry's info)
                using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    writer.Seek(_pHeader->Offset, SeekOrigin.Begin);
                    foreach (var ptr in _entries.Values)
                    {
                        var pTmpEntry = (Entry*)ptr;
                        if (pTmpEntry->UID == pEntry->UID)
                            continue;

                        if (pTmpEntry->Offset > pEntry->Offset)
                            pTmpEntry->Offset -= pEntry->Size;

                        Kernel.memcpy(buffer, pTmpEntry, sizeof(Entry));
                        writer.Write(buffer, 0, sizeof(Entry));
                    }
                    writer.SetLength(size - pEntry->Size - sizeof(Entry));
                }

                //Remove the entry from the dictionnary.
                _entries.Remove(id);

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
        public bool UpdateEntry(uint id, string source)
        {
            lock (_entries)
            {
                if (!_entries.ContainsKey(id))
                    return false;

                if (!File.Exists(source))
                    return false;

                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];

                var info = new FileInfo(source);
                var pEntry = (Entry*)_entries[id];

                //Move all the data to make place
                var tmpPath = Path.GetTempFileName();
                using (var reader = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(tmpPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        reader.Seek(pEntry->Offset + pEntry->Size, SeekOrigin.Begin);
                        writer.Seek(0, SeekOrigin.Begin);

                        var read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (read > 0)
                        {
                            writer.Write(buffer, 0, read);
                            read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                using (var reader = new FileStream(tmpPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        reader.Seek(0, SeekOrigin.Begin);
                        writer.Seek(pEntry->Offset + info.Length, SeekOrigin.Begin);

                        var read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (read > 0)
                        {
                            writer.Write(buffer, 0, read);
                            read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                File.Delete(tmpPath);

                //Write new data
                using (var reader = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    {
                        writer.Seek(pEntry->Offset, SeekOrigin.Begin);

                        var read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (read > 0)
                        {
                            writer.Write(buffer, 0, read);
                            read = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }

                //Change all the offset & size & truncate if required
                using (var writer = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    _pHeader->Offset = (uint)(_pHeader->Offset + (info.Length - pEntry->Size));
                    Kernel.memcpy(buffer, _pHeader, sizeof(Header));
                    writer.Write(buffer, 0, sizeof(Header));

                    writer.Seek(_pHeader->Offset, SeekOrigin.Begin);
                    foreach (var ptr in _entries.Values)
                    {
                        var pTmpEntry = (Entry*)ptr;
                        if (pTmpEntry->UID == pEntry->UID)
                            continue;

                        if (pTmpEntry->Offset > pEntry->Offset)
                            pTmpEntry->Offset = (uint)(pTmpEntry->Offset + (info.Length - pEntry->Size));

                        Kernel.memcpy(buffer, pTmpEntry, sizeof(Entry));
                        writer.Write(buffer, 0, sizeof(Entry));
                    }

                    var oldSize = pEntry->Size;
                    pEntry->Size = (uint)info.Length;

                    Kernel.memcpy(buffer, pEntry, sizeof(Entry));
                    writer.Write(buffer, 0, sizeof(Entry));

                    if (info.Length < oldSize)
                        writer.SetLength(new FileInfo(_filename).Length - (oldSize - info.Length));
                }

                return true;
            }
        }

        /// <summary>
        /// Pack the folder pointed by the path (source) in a package pointed by the other path (destination).
        /// A list containing all the unique IDs and the original value will be produced.
        /// </summary>
        public static void Pack(string source, string destination)
        {
            if (!destination.EndsWith(".wdf"))
                destination += ".wdf";

            var di = new DirectoryInfo(source);
            var files = di.GetFiles("*.*", SearchOption.AllDirectories);

            //Really safe implementation...
            var maxTotalDataSize = (uint)(uint.MaxValue - sizeof(Header) - files.Length * sizeof(Entry));
            uint totalDataSize = 0;

            var offsets = new uint[files.Length];
            for (var i = 0; i < files.Length; i++)
            {
                if (files[i].Length > uint.MaxValue)
                    throw new Exception(files[i].FullName + " is bigger than 4 Gio. It is not supported by the format.");

                if (maxTotalDataSize - totalDataSize < files[i].Length)
                    throw new Exception("The folder can't be packed because it is bigger than the 4 Gio that are possible.");

                offsets[i] = (uint)(sizeof(Header) + totalDataSize);
                totalDataSize += (uint)files[i].Length;
            }
            //End.

            Header* pHeader = stackalloc Header[1];
            pHeader->Id = WdfId;
            pHeader->Number = files.Length;
            pHeader->Offset = (uint)(sizeof(Header) + totalDataSize);

            var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
            using (var fStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Console.Write("Writing header... ");
                Kernel.memcpy(buffer, pHeader, sizeof(Header));
                fStream.Write(buffer, 0, sizeof(Header));
                Console.WriteLine("Ok!");

                Console.Write("Writing data... ");
                for (var i = 0; i < pHeader->Number; i++)
                {
                    Console.Write("\rWriting data... {0}%", i * 100 / pHeader->Number);

                    using (var reader = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var length = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        while (length > 0)
                        {
                            fStream.Write(buffer, 0, length);
                            length = reader.Read(buffer, 0, Kernel.MAX_BUFFER_SIZE);
                        }
                    }
                }
                Console.WriteLine("\b\b\bOk!");

                Console.Write("Writing entries... ");
                using (var writer = new StreamWriter(destination.Replace(".wdf", ".lst"), false, Encoding.Default))
                {
                    Entry* pEntry = stackalloc Entry[1];
                    for (var i = 0; i < pHeader->Number; i++)
                    {
                        Console.Write("\rWriting entries... {0}%", i * 100 / pHeader->Number);

                        var relativePath = files[i].FullName.Replace(di.Parent?.FullName + "\\", "");
                        relativePath = relativePath.ToLowerInvariant();
                        relativePath = relativePath.Replace('/', '\\');

                        var uid = String2Id(relativePath);
                        writer.WriteLine("{0}={1}", uid, relativePath);

                        pEntry->UID = uid;
                        pEntry->Offset = offsets[i];
                        pEntry->Size = (uint)files[i].Length;
                        pEntry->Space = 0;

                        Kernel.memcpy(buffer, pEntry, sizeof(Entry));
                        fStream.Write(buffer, 0, sizeof(Entry));
                    }
                }
                Console.WriteLine("\b\b\bOk!");
            }
        }

        /// <summary>
        /// Convert the specified string to its unique ID.
        /// It have to be used to retreive handle entries.
        /// </summary>
        public static uint String2Id(string str)
        {
            //x86 - 32 bits - Registers
            uint ecx;

            int i;
            uint* m = stackalloc uint[0x46];
            Kernel.memset(m, 0, sizeof(uint) * 0x46);
            byte* buffer = stackalloc byte[0x100];
            Kernel.memset(buffer, 0, 0x100);

            str = str.ToLowerInvariant();
            str = str.Replace('\\', '/');

            for (i = 0; i < str.Length; i++)
                buffer[i] = (byte)str[i];

            var length = str.Length % 4 == 0 ? str.Length / 4 : str.Length / 4 + 1;
            for (i = 0; i < length; i++)
                m[i] = *((uint*)buffer + i);
            m[i++] = 0x9BE74448;
            m[i++] = 0x66F42C48;

            var v = 0xF4FA8928;

            uint edi = 0x7758B42B;
            uint esi = 0x37A8470E;

            for (ecx = 0; ecx < i; ecx++)
            {
                uint ebx = 0x267B0B11;
                v = (v << 1) | (v >> 0x1F);
                ebx ^= v;
                var eax = m[ecx];
                esi ^= eax;
                edi ^= eax;
                var edx = ebx;
                edx += edi;
                edx |= 0x02040801;
                edx &= 0xBFEF7FDF;
                ulong num = edx;
                num *= esi;
                eax = (uint)num;
                edx = (uint)(num >> 0x20);
                if (edx != 0)
                    eax++;
                num = eax;
                num += edx;
                eax = (uint)num;
                if ((uint)(num >> 0x20) != 0)
                    eax++;
                edx = ebx;
                edx += esi;
                edx |= 0x00804021;
                edx &= 0x7DFEFBFF;
                esi = eax;
                num = edi;
                num *= edx;
                eax = (uint)num;
                edx = (uint)(num >> 0x20);
                num = edx;
                num += edx;
                edx = (uint)num;
                if ((uint)(num >> 0x20) != 0)
                    eax++;
                num = eax;
                num += edx;
                eax = (uint)num;
                if ((uint)(num >> 0x20) != 0)
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