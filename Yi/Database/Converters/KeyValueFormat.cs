using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Yi.Database.Converters
{
    public class KeyValueFormat : IDisposable
    {
        internal string Filename { get; set; }
        internal string Splitter { get; set; }
        internal Dictionary<string, object> KeyValue { get; set; }
        internal HashSet<string> LookupTable { get; set; }

        /// <summary>
        /// Initializes KeyValueFormat values.
        /// Generates KeyValueFormat file in memory for handling.
        /// </summary>
        /// <param name="filename">The file to load key/value pairs from.</param>
        /// <param name="kvsplit">split sequence</param>
        public KeyValueFormat(string filename, string kvsplit = "=")
        {
            if (!File.Exists(filename))
                File.Create(filename).Close();
            Splitter = kvsplit;
            Filename = filename;
            KeyValue = new Dictionary<string, object>(StringComparer.Ordinal);
            LookupTable = new HashSet<string>(StringComparer.Ordinal);
            Generate();
        }

        /// <summary>Writes a value to the specified key (caller).</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="caller">The calling member to write as key.</param>
        public void MagicSave(object value, [CallerMemberName] string caller = null)
        {
            if (caller == null)
                throw new ArgumentNullException(nameof(caller));

            if (Contains(caller))
                KeyValue[caller] = value;
            else
            {
                LookupTable.Add(caller);
                KeyValue.Add(caller, value);
            }
        }

        /// <summary>Writes a value using the specified value to the specified key.</summary>
        /// <param name="key">The key containg the value.</param>
        /// <param name="value">The value to write.</param>
        public void Save(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (Contains(key))
                KeyValue[key] = value;
            else
            {
                LookupTable.Add(key);
                KeyValue.Add(key, value);
            }
        }

        public T MagicLoad<T>([CallerMemberName] string key = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (Contains(key))
                return (T)Convert.ChangeType(KeyValue[key], typeof(T));

            KeyValue.Add(key, default(T));
            LookupTable.Add(key);
            return default(T);
        }

        public T MagicLoad<T>(T defaultval, [CallerMemberName] string key = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (Contains(key))
                return (T)Convert.ChangeType(KeyValue[key], typeof(T));

            KeyValue.Add(key, defaultval);
            LookupTable.Add(key);
            return defaultval;
        }

        /// <summary>Reads a value from the speified key. Returns as specified data Action (T).</summary>
        /// <typeparam name="T">The data Action to return as.</typeparam>
        /// <param name="key">The key to read from.</param>
        public T Load<T>(string key)
        {
            if (Contains(key))
                return (T)Convert.ChangeType(KeyValue[key], typeof(T));

            LookupTable.Add(key);
            KeyValue.Add(key, default(T));
            return default(T);
        }

        public T Load<T>(string key, T defaultval)
        {
            if (Contains(key))
                return (T)Convert.ChangeType(KeyValue[key], typeof(T));

            KeyValue.Add(key, default(T));
            LookupTable.Add(key);
            return defaultval;
        }

        /// <summary>Generates the file for handling in memory.</summary>
        public void Generate()
        {
            using (var stream = File.OpenText(Filename))
            {
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    if (line == null)
                        continue;
                    var keyvalue = line.Split(new[] {Splitter}, StringSplitOptions.RemoveEmptyEntries);
                    if (keyvalue.Length < 2)
                        continue;

                    KeyValue.Add(keyvalue[0], keyvalue[1]);
                    LookupTable.Add(keyvalue[0]);
                }
            }
        }

        public bool Contains(string value) => Enumerable.Contains(LookupTable, value);

        /// <summary>Flushes the cache to the hard drive.</summary>
        public void Commit()
        {
            using (var stream = File.Open(Filename, FileMode.Truncate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("--- KeyValueFormat AniFile Format 1.3 by Yuki with help of Carl Leth (http://bit.ly/1QvTto7) ---");
                foreach (var kvp in KeyValue)
                    writer.WriteLine("{0}{1}{2}", kvp.Key, Splitter, kvp.Value);
            }
        }

        /// <summary>Rebuilds the KeyValueFormat cache.</summary>
        public void RefreshCache()
        {
            KeyValue.Clear();
            LookupTable.Clear();
            Generate();
        }

        public void Dispose()
        {
            KeyValue.Clear();
            LookupTable.Clear();
            Filename = null;
            Splitter = null;
            KeyValue = null;
            LookupTable = null;
        }
    }
}