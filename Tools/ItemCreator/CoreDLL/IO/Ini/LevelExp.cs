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

namespace ItemCreator.CoreDLL.IO.Ini
{
    /// <summary>
    /// LevelExp
    /// </summary>
    public unsafe class LevelExp
    {
        private int[] _entries;

        /// <summary>
        /// Create a new LevelExp instance to handle the TQ's LevelExp file.
        /// </summary>
        public LevelExp()
        {
            _entries = new int[0];
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
            if (_entries == null) return;
            lock (_entries)
            {
                _entries = new int[0];
            }
        }

        /// <summary>
        /// Load the specified levelexp file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(string path)
        {
            Clear();

            lock (_entries)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[Kernel.MAX_BUFFER_SIZE];

                    var read = stream.Read(buffer, 0, buffer.Length);
                    var last = 0;
                    while (read > 0)
                    {
                        var newLen = _entries.Length + read / 4;
                        var temp = new int[newLen];

                        _entries.CopyTo(temp, 0);
                        _entries = temp;

                        fixed (byte* pBuffer = buffer)
                        {
                            int i;
                            for (i = 0; i < read / 4; i++)
                                _entries[i + last] = Math.Abs(*((int*)pBuffer + i));
                            last += i + 1;
                        }
                        read = stream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Load the specified levelexp file (in custom text format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(string path)
        {
            Clear();

            lock (_entries)
            {
                var lines = File.ReadAllLines(path, Encoding.GetEncoding("Windows-1252"));

                _entries = new int[lines.Length];
                for (var i = 0; i < lines.Length; i++)
                {
                    int.TryParse(lines[i], out int exp);

                    _entries[i] = exp;
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified levelexp file (in binary format).
        /// </summary>
        public void SaveToDat(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                int[] values;

                lock (_entries)
                {
                    values = new int[_entries.Length];
                    _entries.CopyTo(values, 0);
                }

                int* pValue = stackalloc int[1];
                foreach (var t in values)
                {
                    *pValue = t * -1;

                    Kernel.memcpy(buffer, pValue, sizeof(int));
                    stream.Write(buffer, 0, sizeof(int));
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified levelexp file (in custom text format).
        /// </summary>
        public void SaveToTxt(string path)
        {
            using (var stream = new StreamWriter(path, false, Encoding.GetEncoding("Windows-1252")))
            {
                lock (_entries)
                {
                    foreach (var exp in _entries)
                        stream.WriteLine(exp);
                }
            }
        }

        /// <summary>
        /// Get the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count => _entries.Length;

        /// <summary>
        /// Get an array containing the values of the dictionary.
        /// </summary>
        public int[] Values
        {
            get
            {
                lock (_entries)
                {
                    var values = new int[_entries.Length];
                    _entries.CopyTo(values, 0);
                    return values;
                }
            }
        }

        /// <summary>
        /// Determine whether the dictionary contains the specified key.
        /// </summary>
        public bool ContainsKey(int level)
        {
            if (level < 1)
                return false;

            lock (_entries)
            {
                return _entries.Length >= level;
            }
        }

        /// <summary>
        /// Get the information of the specified level.
        /// </summary>
        public bool TryGetValue(int level, out int exp)
        {
            exp = 0;

            if (level < 1)
                return false;

            lock (_entries)
            {
                if (_entries.Length < level)
                    return false;

                exp = _entries[level - 1];
                return true;
            }
        }

        /// <summary>
        /// Add the levelexp's information in the dictionary.
        /// It will work only if the level is next to the last one.
        /// </summary>
        public bool Add(int level, int exp)
        {
            if (level < 1)
                return false;

            lock (_entries)
            {
                if (_entries.Length + 1 != level)
                    return false;

                var temp = new int[_entries.Length + 1];
                Array.Copy(_entries, 0, temp, 0, _entries.Length);
                _entries = temp;

                _entries[level - 1] = exp;
                return true;
            }
        }

        /// <summary>
        /// Delete the levelexp's information in the dictionary.
        /// It will work only for the last one.
        /// </summary>
        public bool Remove(int level)
        {
            if (level < 1)
                return false;

            lock (_entries)
            {
                if (_entries.Length != level)
                    return false;

                var temp = new int[_entries.Length - 1];
                Array.Copy(_entries, 0, temp, 0, temp.Length);
                _entries = temp;
                return true;
            }
        }

        /// <summary>
        /// Update the levelexp's information in the dictionary.
        /// </summary>
        public bool Update(int level, int exp)
        {
            if (level < 1)
                return false;

            lock (_entries)
            {
                if (_entries.Length < level)
                    return false;

                _entries[level - 1] = exp;
                return true;
            }
        }
    }
}

// * ************************************************************
// * * END:                                         levelexp.cs *
// * ************************************************************