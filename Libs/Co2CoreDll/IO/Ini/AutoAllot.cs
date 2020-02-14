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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// AutoAllot
    /// </summary>
    public unsafe class AutoAllot
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public Int32 Amount;
            public Int32 Level;
            public fixed Int32 Professions[1]; //Profession / 10
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public Int32 Strength;
            public Int32 Agility;
            public Int32 Vitality;
            public Int32 Spirit;
        };

        //Entries[Profession][Level] = Stat;
        private Dictionary<Int32, IntPtr[]> Entries = null;

        /// <summary>
        /// Create a new LevelExp instance to handle the TQ's AutoAllot file.
        /// </summary>
        public AutoAllot()
        {
            this.Entries = new Dictionary<Int32, IntPtr[]>();
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
            lock (Entries)
            {
                foreach (var Value in Entries.Values)
                {
                    for (var i = 0; i < Value.Length; i++)
                        Kernel.free((void*)Value[i]);
                }
                Entries.Clear();
            }
        }

        /// <summary>
        /// Load the specified autoallot file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(String Path)
        {
            Clear();

            lock (Entries)
            {
                using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                    Stream.Read(Buffer, 0, sizeof(Int32));

                    var Amount = 0;
                    fixed (Byte* pBuffer = Buffer)
                        Amount = *((Int32*)pBuffer);
                    var Length = (Amount + 2) * sizeof(Int32);

                    var pHeader = (Header*)Kernel.malloc(Length);
                    Stream.Seek(0, SeekOrigin.Begin);
                    Stream.Read(Buffer, 0, Length);
                    Kernel.memcpy(pHeader, Buffer, Length);

                    Entries = new Dictionary<Int32, IntPtr[]>(Amount);
                    for (var i = 0; i < pHeader->Amount; i++)
                    {
                        Entries.Add(pHeader->Professions[i], new IntPtr[pHeader->Level]);
                        for (var j = 0; j < pHeader->Level; j++)
                        {
                            var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));
                            Stream.Read(Buffer, 0, sizeof(Entry));
                            Kernel.memcpy(pEntry, Buffer, sizeof(Entry));

                            Entries[pHeader->Professions[i]][j] = (IntPtr)pEntry;
                        }
                    }
                    Kernel.free(pHeader);
                }
            }
        }

        /// <summary>
        /// Load the specified autoallot file (in custom text format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save all the dictionary to the specified autoallot file (in binary format).
        /// </summary>
        public void SaveToDat(String Path)
        {
            using (var Stream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                lock (Entries)
                {
                    var Length = (Entries.Count + 2) * sizeof(Int32);

                    var pHeader = (Header*)Kernel.malloc(Length);
                    pHeader->Amount = Entries.Count;

                    pHeader->Level = Int32.MaxValue;
                    foreach (var Stats in Entries.Values)
                        pHeader->Level = Math.Min(pHeader->Level, Stats.Length);

                    var i = 0;
                    foreach (var Profession in Entries.Keys)
                    {
                        pHeader->Professions[i] = Profession;
                        i++;
                    }

                    Kernel.memcpy(Buffer, pHeader, Length);
                    Stream.Write(Buffer, 0, Length);

                    foreach (var Stats in Entries.Values)
                    {
                        for (i = 0; i < pHeader->Level; i++)
                        {
                            Kernel.memcpy(Buffer, (Entry*)Stats[i], sizeof(Entry));
                            Stream.Write(Buffer, 0, sizeof(Entry));
                        }
                    }
                    Kernel.free(pHeader);
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified autoallot file (in custom text format).
        /// </summary>
        public void SaveToTxt(String Path)
        {
            using (var Stream = new StreamWriter(Path, false, Encoding.GetEncoding("Windows-1252")))
            {
                lock (Entries)
                {
                    var Level = Int32.MaxValue;
                    foreach (var Stats in Entries.Values)
                        Level = Math.Min(Level, Stats.Length);

                    Stream.WriteLine("MAX_LEVEL={0}", Level);
                    Stream.WriteLine();

                    foreach (var KV in Entries)
                    {
                        Stream.WriteLine("[PROFESSION{0}]", KV.Key);
                        for (var i = 0; i < Level; i++)
                        {
                            Stream.WriteLine("Strength[{0}]={1}", i, ((Entry*)KV.Value[i])->Strength);
                            Stream.WriteLine("Agility[{0}]={1}", i, ((Entry*)KV.Value[i])->Agility);
                            Stream.WriteLine("Vitality[{0}]={1}", i, ((Entry*)KV.Value[i])->Vitality);
                            Stream.WriteLine("Spirit[{0}]={1}", i, ((Entry*)KV.Value[i])->Spirit);
                        }
                        Stream.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Get the information of the specified profession for the specified level.
        /// </summary>
        public Boolean GetAutoAllotInfo(Int32 Profession, Int32 Level, ref Entry Entry)
        {
            Entry = new Entry();

            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.ContainsKey(Profession))
                {
                    if (Entries[Profession].Length < Level)
                    {
                        fixed (Entry* pEntry = &Entry)
                            Kernel.memcpy(pEntry, (Entry*)(Entries[Profession][Level - 1]), sizeof(Entry));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Add the autoallot's information in the dictionary.
        /// It can be used to create an editor or an temp autoallot.
        /// </summary>
        public Boolean AddAutoAllotInfo(Int32 Profession, Int32 Level, Entry Entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the autoallot's information in the dictionary.
        /// </summary>
        public Boolean DelAutoAllotInfo(Int32 MagicType, UInt32 Level)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the autoallot's information in the dictionary.
        /// </summary>
        public Boolean UpdAutoAllotInfo(Int32 MagicType, UInt32 Level, Entry Entry)
        {
            throw new NotImplementedException();
        }
    }
}

// * ************************************************************
// * * END:                                        autoallot.cs *
// * ************************************************************