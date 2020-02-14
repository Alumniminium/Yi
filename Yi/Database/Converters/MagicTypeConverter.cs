using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Yi.Enums;
using Yi.Helpers;
using Yi.Structures;

namespace Yi.Database.Converters
{
    public static class MagicTypeConverter
    {
        public static Task Load()
        {
            return Task.Run(() =>
            {
                var magicType = new MagicType();
                magicType.LoadFromDat("RAW/ini/MagicType.dat");
                foreach (var value in magicType.Values)
                {
                    var entry = new MagicTypeEntry {Level = (byte)value.Level, Target = value.Target, ActiveTime = value.ActiveTime, AutoActive = Convert.ToBoolean(value.AutoActive), AutoLearn = Convert.ToBoolean(value.AutoLearn), CanBeusedInMarket = Convert.ToBoolean(value.CanBeusedInMarket), Crime = Convert.ToBoolean(value.Crime), Delay = value.Delay, Distance = value.Distance, ExpRequired = value.ExpRequired, Ground = Convert.ToBoolean(value.Ground), IntoneDuration = value.IntoneDuration, LearnLevel = Convert.ToByte(value.LearnLevel), LevelRequired = Convert.ToByte(value.LevelRequired), MpCost = value.MpCost, StaminaCost = Convert.ToByte(value.UsePP), Multi = value.Multi, Power = (int)value.Power, Success = Convert.ToByte(value.Success), TargetDelay = value.TargetDelay, TargetWoundDelay = value.TargetWoundDelay, Type = (MagicTypeEnum)value.Type, Time = value.Time, WeaponHit = Convert.ToBoolean(value.WeaponHit), WeaponSubType = Convert.ToUInt16(value.WeaponSubType), Xp = Convert.ToByte(value.Xp)};

                    Collections.Skills.AddOrUpdate(value.MagicType * 10 + entry.Level, entry);
                }
            });
        }
    }

    public unsafe class MagicType
    {
        public const int MAX_NAMESIZE = 16;
        public const int MAX_DESCSIZE = 64;
        public const int MAX_DESCEXSIZE = 256;
        public const int MAX_EFFECTSIZE = 64;
        public const int MAX_SOUNDSIZE = 260;

        //Constants for magic's action sort field.
        public const uint ATTACK_SINGLE_HP = 1;
        public const uint RECOVER_SINGLE_HP = 2;
        public const uint ATTACK_CROSS_HP = 3;
        public const uint ATTACK_SECTOR_HP = 4;
        public const uint ATTACK_ROUND_HP = 5;
        public const uint ATTACK_SINGLE_STATUS = 6;
        public const uint RECOVER_SINGLE_STATUS = 7;
        public const uint SQUARE = 8;
        public const uint JUMPATTACK = 9;
        public const uint RANDOMTRANS = 10;
        public const uint DISPATCHXP = 11;
        public const uint COLLIDE = 12;
        public const uint SERIALCUT = 13;
        public const uint LINE = 14;
        public const uint ATKRANGE = 15;
        public const uint ATKSTATUS = 16;
        public const uint CALL_TEAMMEMBER = 17;
        public const uint RECORDTRANSSPELL = 18; // record map position to trans spell.
        public const uint TRANSFORM = 19;
        public const uint ADDMANA = 20; // support self target only.
        public const uint LAYTRAP = 21;
        public const uint DANCE = 22;
        public const uint CALLPET = 23;
        public const uint VAMPIRE = 24; //power is percent award. use for call pet
        public const uint INSTEAD = 25; //use for call pet
        public const uint DECLIFE = 26;
        public const uint GROUNDSTING = 27;
        public const uint REBORN = 28;
        public const uint TEAM_MAGIC = 29;
        public const uint BOMB_LOCKALL = 30;
        public const uint SORB_SOUL = 31;
        public const uint STEAL = 32;
        public const uint LINE_PENETRABLE = 33;
        public const uint BLAST_THUNDER = 34;
        public const uint MULTI_ATTACHSTATUS = 35;
        public const uint MULTI_DETACHSTATUS = 36;
        public const uint MULTI_CURE = 37;
        public const uint STEAL_MONEY = 38;
        public const uint KO = 39;
        public const uint ESCAPE = 40;
        public const uint FLASH_ATTACK = 41;
        public const uint ATTRACK_MONSTER = 42;

        //Constants for magic's target field.
        public const uint TARGET_SELF = 1;
        public const uint TARGET_NONE = 2;
        public const uint TARGET_TERRAIN = 4;
        public const uint PASSIVE = 8;
        public const uint TARGET_BODY = 16;

        //Constants for magic's xp field.
        public const uint MAGIC = 0;
        public const uint XPSKILL = 1;
        public const uint KONGFU = 2;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public int Amount;
            public fixed int UIDs [1]; //Action * 10 + Level
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public readonly int MagicType;
            public readonly uint Type;
            public fixed byte Name [MAX_NAMESIZE];
            public readonly uint Crime;
            public readonly uint Ground;
            public readonly uint Multi;
            public readonly uint Target;
            public readonly uint Level;
            public readonly uint MpCost;
            public readonly uint Power;
            public readonly uint IntoneDuration;
            public readonly uint Success;
            public readonly uint Time;
            public readonly uint Range; //X= %100, y= %10000 - x*100
            public readonly uint Distance;
            public readonly uint Status;
            public readonly uint ProfessionalRequired;
            public readonly uint ExpRequired;
            public readonly uint LevelRequired;
            public readonly uint Xp;
            public readonly uint WeaponSubType;
            public readonly uint ActiveTime;
            public readonly uint AutoActive;
            public readonly uint FloorAttribute;
            public readonly uint AutoLearn;
            public readonly uint LearnLevel;
            public readonly uint DropWeapon;
            public readonly uint UsePP;
            public readonly uint WeaponHit;
            public readonly int UseItem;
            public readonly int NextMagic;
            public readonly uint Delay;
            public readonly uint UseItemNum;
            public readonly uint SenderAction;
            public fixed byte Desc [MAX_DESCSIZE];
            public fixed byte DescEx [MAX_DESCEXSIZE];
            public fixed byte IntoneEffect [MAX_EFFECTSIZE];
            public fixed byte IntoneSound [MAX_SOUNDSIZE];
            public fixed byte SenderEffect [MAX_EFFECTSIZE];
            public fixed byte SenderSound [MAX_SOUNDSIZE];
            public readonly uint TargetDelay;
            public fixed byte TargetEffect [MAX_EFFECTSIZE];
            public fixed byte TargetSound [MAX_SOUNDSIZE];
            public fixed byte GroundEffect [MAX_EFFECTSIZE];
            public fixed byte TraceEffect [MAX_EFFECTSIZE];
            public readonly uint ScreenRepresent;
            public readonly uint CanBeusedInMarket;
            public readonly uint TargetWoundDelay;

            public string GetName()
            {
                fixed (byte* sb = Name)
                    return new string((sbyte*)sb);
            }
        };

        private Dictionary<int, IntPtr> _entries;

        /// <summary>
        /// CreateFor a new MagicType instance to handle the TQ's MagicType file.
        /// </summary>
        public MagicType()
        {
            _entries = new Dictionary<int, IntPtr>();
        }

        ~MagicType()
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
                foreach (var ptr in _entries.Values)
                    NativeMethods.Free((Entry*)ptr);
            }
            lock (_entries)
            {
                _entries.Clear();
            }
        }

        public void LoadFromDat(string path)
        {
            Clear();

            lock (_entries)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[4096];
                    stream.Read(buffer, 0, sizeof(int));

                    int amount;
                    fixed (byte* pBuffer = buffer)
                        amount = *(int*)pBuffer;
                    var length = (amount + 1) * sizeof(int);

                    var pHeader = (Header*)NativeMethods.Malloc(length);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, length);
                    NativeMethods.Memcpy((byte*)pHeader, buffer, length);

                    _entries = new Dictionary<int, IntPtr>(amount);
                    for (var i = 0; i < amount; i++)
                    {
                        var pEntry = (Entry*)NativeMethods.Malloc(sizeof(Entry));

                        stream.Read(buffer, 0, sizeof(Entry));
                        NativeMethods.Memcpy((byte*)pEntry, buffer, sizeof(Entry));

                        if (!_entries.ContainsKey(pHeader -> UIDs[i]))
                            _entries.Add(pHeader -> UIDs[i], (IntPtr)pEntry);
                    }
                    NativeMethods.Free(pHeader);
                }
            }
        }

        public void SaveToDat(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var buffer = new byte[4096];
                IntPtr[] pointers;

                lock (_entries)
                {
                    pointers = new IntPtr[_entries.Count];
                    _entries.Values.CopyTo(pointers, 0);
                }

                var length = (pointers.Length + 1) * sizeof(int);

                var pHeader = (Header*)NativeMethods.Malloc(length);
                pHeader -> Amount = pointers.Length;
                for (var i = 0; i < pointers.Length; i++)
                    pHeader -> UIDs[i] = (int)(((Entry*)pointers[i]) -> MagicType * 10 + ((Entry*)pointers[i]) -> Level);

                NativeMethods.Memcpy(buffer, pHeader, length);
                stream.Write(buffer, 0, length);

                for (var i = 0; i < pointers.Length; i++)
                {
                    NativeMethods.Memcpy(buffer, (Entry*)i, sizeof(Entry));
                    stream.Write(buffer, 0, sizeof(Entry));
                }
                NativeMethods.Free(pHeader);
            }
        }

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public int GenUID(Entry entry) => (int)(entry.MagicType * 10 + entry.Level);

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public int GenUID(int magicType, uint level) => (int)(magicType * 10 + level);

        /// <summary>
        /// Get the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Get an array containing the keys of the dictionary.
        /// </summary>
        public int[] Keys
        {
            get
            {
                lock (_entries)
                {
                    var keys = new int[_entries.Count];
                    _entries.Keys.CopyTo(keys, 0);
                    return keys;
                }
            }
        }

        /// <summary>
        /// Get an array containing the values of the dictionary.
        /// </summary>
        public IEnumerable<Entry> Values
        {
            get
            {
                lock (_entries)
                {
                    var values = new Entry[_entries.Count];

                    var i = 0;
                    foreach (var ptr in _entries.Values)
                    {
                        fixed (Entry* pEntry = &values[i])
                            NativeMethods.Memcpy(pEntry, (Entry*)ptr, sizeof(Entry));
                        i++;
                    }
                    return values;
                }
            }
        }

        /// <summary>
        /// Determine whether the dictionary contains the specified key.
        /// </summary>
        public bool ContainsKey(int uniqId)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey(uniqId))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get the information of the specified Action for the specified level.
        /// To get a valid string for some fields, use NativeMethods.cstring function.
        /// </summary>
        public bool TryGetValue(int uniqId, out Entry entry)
        {
            entry = new Entry();
            lock (_entries)
            {
                if (!_entries.ContainsKey(uniqId)) return false;
                fixed (Entry* pEntry = &entry)
                    NativeMethods.Memcpy(pEntry, (Entry*)_entries[uniqId], sizeof(Entry));
                return true;
            }
        }

        /// <summary>
        /// Add the magic's information in the dictionary.
        /// It can be used to create an editor or an temp magic.
        /// </summary>
        public bool Add(Entry entry)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey((int) (entry.MagicType * 10 + entry.Level))) return false;
                var pEntry = (Entry*)NativeMethods.Calloc(sizeof(Entry));
                NativeMethods.Memcpy(pEntry, &entry, sizeof(Entry));

                _entries.Add((int)(pEntry -> MagicType * 10 + pEntry -> Level), (IntPtr)pEntry);
                return true;
            }
        }

        /// <summary>
        /// Delete the magic's information in the dictionary.
        /// </summary>
        public bool Remove(int uniqId)
        {
            lock (_entries)
            {
                if (!_entries.ContainsKey(uniqId)) return false;
                _entries.Remove(uniqId);
                return true;
            }
        }

        /// <summary>
        /// Update the magic's information in the dictionary.
        /// </summary>
        public bool Update(int uniqId, Entry entry)
        {
            lock (_entries)
            {
                if (!_entries.ContainsKey(uniqId)) return false;
                NativeMethods.Memcpy((Entry*)_entries[uniqId], &entry, sizeof(Entry));
                return true;
            }
        }
    }
}