// * ************************************************************
// * * START:                                       levelexp.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * LevelExp class for the library.
// * levelexp.cs
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

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// LevelExp
    /// </summary>
    public unsafe class LevelExp
    {
        private Int32[] Entries = null;

        /// <summary>
        /// Create a new LevelExp instance to handle the TQ's LevelExp file.
        /// </summary>
        public LevelExp()
        {
            this.Entries = new Int32[0];
        }

        ~LevelExp()
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
                    Entries = new Int32[0];
                }
            }
        }

        /// <summary>
        /// Load the specified levelexp file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(String Path)
        {
            Clear();

            lock (Entries)
            {
                using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];

                    var Read = Stream.Read(Buffer, 0, Buffer.Length);
                    var Last = 0;
                    while (Read > 0)
                    {
                        var NewLen = Entries.Length + Read / 4;
                        var Temp = new Int32[NewLen];

                        Entries.CopyTo(Temp, 0);
                        Entries = Temp;

                        fixed (Byte* pBuffer = Buffer)
                        {
                            Int32 i;
                            for (i = 0; i < Read / 4; i++)
                                Entries[i + Last] = Math.Abs(*(((Int32*)pBuffer) + i));
                            Last += i + 1;
                        }
                        Read = Stream.Read(Buffer, 0, Buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified levelexp file (in custom text format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Entries)
            {
                var Lines = File.ReadAllLines(Path, Encoding.GetEncoding("Windows-1252"));

                Entries = new Int32[Lines.Length];
                for (var i = 0; i < Lines.Length; i++)
                {
                    var Exp = 0;
                    Int32.TryParse(Lines[i], out Exp);

                    Entries[i] = Exp;
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified levelexp file (in binary format).
        /// </summary>
        public void SaveToDat(String Path)
        {
            using (var Stream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                var Values = new Int32[0];

                lock (Entries)
                {
                    Values = new Int32[Entries.Length];
                    Entries.CopyTo(Values, 0);
                }

                Int32* pValue = stackalloc Int32[1];
                for (var i = 0; i < Values.Length; i++)
                {
                    *pValue = Values[i] * -1;

                    Kernel.memcpy(Buffer, pValue, sizeof(Int32));
                    Stream.Write(Buffer, 0, sizeof(Int32));
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified levelexp file (in custom text format).
        /// </summary>
        public void SaveToTxt(String Path)
        {
            using (var Stream = new StreamWriter(Path, false, Encoding.GetEncoding("Windows-1252")))
            {
                lock (Entries)
                {
                    foreach (var Exp in Entries)
                        Stream.WriteLine(Exp);
                }
            }
        }

        /// <summary>
        /// Get the number of key/value pairs contained in the dictionary.
        /// </summary>
        public Int32 Count { get { return Entries.Length; } }

        /// <summary>
        /// Get an array containing the values of the dictionary.
        /// </summary>
        public Int32[] Values
        {
            get
            {
                lock (Entries)
                {
                    var Values = new Int32[Entries.Length];
                    Entries.CopyTo(Values, 0);
                    return Values;
                }
            }
        }

        /// <summary>
        /// Determine whether the dictionary contains the specified key.
        /// </summary>
        public Boolean ContainsKey(Int32 Level)
        {
            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.Length < Level)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Get the information of the specified level.
        /// </summary>
        public Boolean TryGetValue(Int32 Level, out Int32 Exp)
        {
            Exp = 0;

            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.Length < Level)
                    return false;

                Exp = Entries[Level - 1];
                return true;
            }
        }

        /// <summary>
        /// Add the levelexp's information in the dictionary.
        /// It will work only if the level is next to the last one.
        /// </summary>
        public Boolean Add(Int32 Level, Int32 Exp)
        {
            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.Length + 1 != Level)
                    return false;

                var temp = new Int32[Entries.Length + 1];
                Array.Copy(Entries, 0, temp, 0, Entries.Length);
                Entries = temp;

                Entries[Level - 1] = Exp;
                return true;
            }
        }

        /// <summary>
        /// Delete the levelexp's information in the dictionary.
        /// It will work only for the last one.
        /// </summary>
        public Boolean Remove(Int32 Level)
        {
            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.Length != Level)
                    return false;

                var temp = new Int32[Entries.Length - 1];
                Array.Copy(Entries, 0, temp, 0, temp.Length);
                Entries = temp;
                return true;
            }
        }

        /// <summary>
        /// Update the levelexp's information in the dictionary.
        /// </summary>
        public Boolean Update(Int32 Level, Int32 Exp)
        {
            if (Level < 1)
                return false;

            lock (Entries)
            {
                if (Entries.Length < Level)
                    return false;

                Entries[Level - 1] = Exp;
                return true;
            }
        }
    }
}

// * ************************************************************
// * * END:                                         levelexp.cs *
// * ************************************************************