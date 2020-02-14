// * ************************************************************
// * * START:                                      magictype.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * MagicType class for the library.
// * maguctype.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (December 27th, 2011)
// * Copyright (C) 2011 CptSky
// *
// * ************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ItemCreator.CoreDLL.IO.Ini
{
    /// <summary>
    /// MagicType
    /// </summary>
    public unsafe class MagicType
    {
        public const int MaxNamesize = 0x10;
        public const int MaxDescsize = 0x40;
        public const int MaxDescexsize = 0x100;
        public const int MaxEffectsize = 0x40;
        public const int MaxSoundsize = 0x104;

        //Constants for magic's action sort field.
        public const uint MagicSortAttackSingleHp			= 1;
        public const uint MagicSortRecoverSingleHp		    = 2;
        public const uint MagicSortAttackCrossHp			= 3;
        public const uint MagicSortAttackSectorHp			= 4;
        public const uint MagicSortAttackRoundHp			= 5;
        public const uint MagicSortAttackSingleStatus		= 6;
        public const uint MagicSortRecoverSingleStatus	    = 7;
        public const uint MagicSortSquare					= 8;
        public const uint MagicSortJumpattack				= 9;
        public const uint MagicSortRandomtrans				= 10;
        public const uint MagicSortDispatchxp				= 11;
        public const uint MagicSortCollide					= 12;
        public const uint MagicSortSerialcut				    = 13;
        public const uint MagicSortLine						= 14;
        public const uint MagicSortAtkrange					= 15;
        public const uint MagicSortAtkstatus				    = 16;
        public const uint MagicSortCallTeammember			= 17;
        public const uint MagicSortRecordtransspell			= 18;	// record map position to trans spell.
        public const uint MagicSortTransform				    = 19;
        public const uint MagicSortAddmana					= 20;	// support self target only.
        public const uint MagicSortLaytrap					= 21;
        public const uint MagicSortDance					    = 22;
        public const uint MagicSortCallpet			        = 23;
        public const uint MagicSortVampire			        = 24;	//power is percent award. use for call pet
        public const uint MagicSortInstead			        = 25;	//use for call pet
        public const uint MagicSortDeclife			        = 26;
        public const uint MagicSortGroundsting		        = 27;
        public const uint MagicSortReborn			        = 28;
        public const uint MagicSortTeamMagic		        = 29;
        public const uint MagicSortBombLockall		        = 30;
        public const uint MagicSortSorbSoul			        = 31;
        public const uint MagicSortSteal				        = 32;
        public const uint MagicSortLinePenetrable	        = 33;
        public const uint MagicSortBlastThunder			    = 34;
        public const uint MagicSortMultiAttachstatus	    = 35;
        public const uint MagicSortMultiDetachstatus	    = 36;
        public const uint MagicSortMultiCure			    = 37;
        public const uint MagicSortStealMoney			    = 38;
        public const uint MagicSortKo					    = 39;
        public const uint MagicSortEscape				    = 40;
        public const uint MagicSortFlashAttack			    = 41;
        public const uint MagicSortAttrackMonster		    = 42;

        //Constants for magic's target field.
        public const uint MagicTargetSelf		    = 0x00000001;
        public const uint MagicTargetNone		    = 0x00000002;
        public const uint MagicTargetTerrain         = 0x00000004;
        public const uint MagicPassive			    = 0x00000008;
        public const uint MagicTargetBody		    = 0x00000010;

        //Constants for magic's xp field.
        public const uint TypeMagic		= 0;
        public const uint TypeXpskill	    = 1;
        public const uint TypeKongfu      = 2;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public int Amount;
            public fixed int UIDs[1]; //Type * 10 + Level
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public int MagicType;
            public uint ActionSort;
            public fixed byte Name[MaxNamesize];
            public uint Crime;
            public uint Ground;
            public uint Multi;
            public uint Target;
            public uint Level;
            public uint MpCost;
            public uint Power;
            public uint IntoneDuration;
            public uint Success;
            public uint Time;
            public uint Range; //X= %100, y= %10000 - x*100
            public uint Distance;
            public uint Status;
            public uint ProfessionalRequired;
            public uint ExpRequired;
            public uint LevelRequired;
            public uint Xp;
            public uint WeaponSubType;
            public uint ActiveTime;
            public uint AutoActive;
            public uint FloorAttribute;
            public uint AutoLearn;
            public uint LearnLevel;
            public uint DropWeapon;
            public uint UsePP;
            public uint WeaponHit;
            public int UseItem;
            public int NextMagic;
            public uint Delay;
            public uint UseItemNum;
            public uint SenderAction;
            public fixed byte Desc[MaxDescsize];
            public fixed byte DescEx[MaxDescexsize];
            public fixed byte IntoneEffect[MaxEffectsize];
            public fixed byte IntoneSound[MaxSoundsize];
            public fixed byte SenderEffect[MaxEffectsize];
            public fixed byte SenderSound[MaxSoundsize];
            public uint TargetDelay;
            public fixed byte TargetEffect[MaxEffectsize];
            public fixed byte TargetSound[MaxSoundsize];
            public fixed byte GroundEffect[MaxEffectsize];
            public fixed byte TraceEffect[MaxEffectsize];
            public uint ScreenRepresent;
            public uint CanBeusedInMarket;
            public uint TargetWoundDelay;
        }

        private Dictionary<int, IntPtr> _entries;

        /// <summary>
        /// Create a new MagicType instance to handle the TQ's MagicType file.
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
                    Kernel.free((Entry*)ptr);
            }
            lock (_entries)
            {
                _entries.Clear();
            }
        }

        /// <summary>
        /// Load the specified magictype file (in binary format) into the dictionary.
        /// </summary>
        public void LoadFromDat(string path)
        {
            Clear();

            lock (_entries)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                    stream.Read(buffer, 0, sizeof(int));

                    int amount;
                    fixed (byte* pBuffer = buffer)
                        amount = *(int*)pBuffer;
                    var length = (amount + 1) * sizeof(int);

                    var pHeader = (Header*)Kernel.malloc(length);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, length);
                    Kernel.memcpy((byte*)pHeader, buffer, length);

                    _entries = new Dictionary<int, IntPtr>(amount);
                    for (var i = 0; i < amount; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));

                        stream.Read(buffer, 0, sizeof(Entry));
                        Kernel.memcpy((byte*)pEntry, buffer, sizeof(Entry));

                        if (!_entries.ContainsKey(pHeader->UIDs[i]))
                            _entries.Add(pHeader->UIDs[i], (IntPtr)pEntry);
                    }
                    Kernel.free(pHeader);
                }
            }
        }

        /// <summary>
        /// Load the specified magictype file (in plain format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(string path)
        {
            Clear();

            lock (_entries)
            {
                using (var stream = new StreamReader(path, Encoding.GetEncoding("Windows-1252")))
                {
                    _entries = new Dictionary<int, IntPtr>();

                    string line;
                    var lineC = 0;
                    while ((line = stream.ReadLine()) != null)
                    {
                        lineC++;

                        var parts = line.Split(' ');
                        var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));

                        try
                        {
                            pEntry->MagicType = int.Parse(parts[0]);
                            pEntry->ActionSort = uint.Parse(parts[1]);
                            var buffer = Encoding.Default.GetBytes(parts[2]);
                            Kernel.memcpy(pEntry->Name, buffer, buffer.Length);
                            pEntry->Crime = uint.Parse(parts[3]);
                            pEntry->Ground = uint.Parse(parts[4]);
                            pEntry->Multi = uint.Parse(parts[5]);
                            pEntry->Target = uint.Parse(parts[6]);
                            pEntry->Level = uint.Parse(parts[7]);
                            pEntry->MpCost = uint.Parse(parts[8]);
                            pEntry->Power = uint.Parse(parts[9]);
                            pEntry->IntoneDuration = uint.Parse(parts[10]);
                            pEntry->Success = uint.Parse(parts[11]);
                            pEntry->Time = uint.Parse(parts[12]);
                            pEntry->Range = uint.Parse(parts[13]);
                            pEntry->Distance = uint.Parse(parts[14]);
                            pEntry->Status = uint.Parse(parts[15]);
                            pEntry->ProfessionalRequired = uint.Parse(parts[16]);
                            pEntry->ExpRequired = uint.Parse(parts[17]);
                            pEntry->LevelRequired = uint.Parse(parts[18]);
                            pEntry->Xp = uint.Parse(parts[19]);
                            pEntry->WeaponSubType = uint.Parse(parts[20]);
                            pEntry->ActiveTime = uint.Parse(parts[21]);
                            pEntry->AutoActive = uint.Parse(parts[22]);
                            pEntry->FloorAttribute = uint.Parse(parts[23]);
                            pEntry->AutoLearn = uint.Parse(parts[24]);
                            pEntry->LearnLevel = uint.Parse(parts[25]);
                            pEntry->DropWeapon = uint.Parse(parts[26]);
                            pEntry->UsePP = uint.Parse(parts[27]);
                            pEntry->WeaponHit = uint.Parse(parts[28]);
                            pEntry->UseItem = int.Parse(parts[29]);
                            pEntry->NextMagic = int.Parse(parts[30]);
                            pEntry->Delay = uint.Parse(parts[31]);
                            pEntry->UseItemNum = uint.Parse(parts[32]);
                            pEntry->SenderAction = uint.Parse(parts[33]);
                            buffer = Encoding.Default.GetBytes(parts[34]);
                            Kernel.memcpy(pEntry->Desc, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[35]);
                            Kernel.memcpy(pEntry->DescEx, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[36]);
                            Kernel.memcpy(pEntry->IntoneEffect, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[37]);
                            Kernel.memcpy(pEntry->IntoneSound, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[38]);
                            Kernel.memcpy(pEntry->SenderEffect, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[39]);
                            Kernel.memcpy(pEntry->SenderSound, buffer, buffer.Length);
                            pEntry->TargetDelay = uint.Parse(parts[40]);
                            buffer = Encoding.Default.GetBytes(parts[41]);
                            Kernel.memcpy(pEntry->TargetEffect, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[42]);
                            Kernel.memcpy(pEntry->TargetSound, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[43]);
                            Kernel.memcpy(pEntry->GroundEffect, buffer, buffer.Length);
                            buffer = Encoding.Default.GetBytes(parts[44]);
                            Kernel.memcpy(pEntry->TraceEffect, buffer, buffer.Length);
                            pEntry->ScreenRepresent = uint.Parse(parts[45]);
                            pEntry->CanBeusedInMarket = uint.Parse(parts[46]);
                            pEntry->TargetWoundDelay = uint.Parse(parts[47]);

                            if (!_entries.ContainsKey((int)(pEntry->MagicType * 10 + pEntry->Level)))
                                _entries.Add((int)(pEntry->MagicType * 10 + pEntry->Level), (IntPtr)pEntry);
                        }
                        catch (Exception exc)
                        { 
                            Console.WriteLine("Error at line {0}.\n{1}", lineC, exc);
                            Kernel.free(pEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified magictype file (in binary format).
        /// </summary>
        public void SaveToDat(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                IntPtr[] pointers;

                lock (_entries)
                {
                    pointers = new IntPtr[_entries.Count];
                    _entries.Values.CopyTo(pointers, 0);
                }

                var length = (pointers.Length + 1) * sizeof(int);

                var pHeader = (Header*)Kernel.malloc(length);
                pHeader->Amount = pointers.Length;
                for (var i = 0; i < pointers.Length; i++)
                    pHeader->UIDs[i] = (int)(((Entry*)pointers[i])->MagicType * 10 + ((Entry*)pointers[i])->Level);

                Kernel.memcpy(buffer, pHeader, length);
                stream.Write(buffer, 0, length);

                foreach (var t in pointers)
                {
                    Kernel.memcpy(buffer, (Entry*)t, sizeof(Entry));
                    stream.Write(buffer, 0, sizeof(Entry));
                }
                Kernel.free(pHeader);
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified magictype file (in plain format).
        /// </summary>
        public void SaveToTxt(string path)
        {
            using (var stream = new StreamWriter(path, false, Encoding.GetEncoding("Windows-1252")))
            {
                IntPtr[] pointers;

                lock (_entries)
                {
                    pointers = new IntPtr[_entries.Count];
                    _entries.Values.CopyTo(pointers, 0);
                }

                foreach (var t in pointers)
                {
                    var pEntry = (Entry*)t;

                    var builder = new StringBuilder(Kernel.MAX_BUFFER_SIZE);
                    builder.Append(pEntry->MagicType + " ");
                    builder.Append(pEntry->ActionSort + " ");
                    builder.Append(Kernel.cstring(pEntry->Name, MaxNamesize) + " ");
                    builder.Append(pEntry->Crime + " ");
                    builder.Append(pEntry->Ground + " ");
                    builder.Append(pEntry->Multi + " ");
                    builder.Append(pEntry->Target + " ");
                    builder.Append(pEntry->Level + " ");
                    builder.Append(pEntry->MpCost + " ");
                    builder.Append(pEntry->Power + " ");
                    builder.Append(pEntry->IntoneDuration + " ");
                    builder.Append(pEntry->Success + " ");
                    builder.Append(pEntry->Time + " ");
                    builder.Append(pEntry->Range + " ");
                    builder.Append(pEntry->Distance + " ");
                    builder.Append(pEntry->Status + " ");
                    builder.Append(pEntry->ProfessionalRequired + " ");
                    builder.Append(pEntry->ExpRequired + " ");
                    builder.Append(pEntry->LevelRequired + " ");
                    builder.Append(pEntry->Xp + " ");
                    builder.Append(pEntry->WeaponSubType + " ");
                    builder.Append(pEntry->ActiveTime + " ");
                    builder.Append(pEntry->AutoActive + " ");
                    builder.Append(pEntry->FloorAttribute + " ");
                    builder.Append(pEntry->AutoLearn + " ");
                    builder.Append(pEntry->LearnLevel + " ");
                    builder.Append(pEntry->DropWeapon + " ");
                    builder.Append(pEntry->UsePP + " ");
                    builder.Append(pEntry->WeaponHit + " ");
                    builder.Append(pEntry->UseItem + " ");
                    builder.Append(pEntry->NextMagic + " ");
                    builder.Append(pEntry->Delay + " ");
                    builder.Append(pEntry->UseItemNum + " ");
                    builder.Append(pEntry->SenderAction + " ");
                    builder.Append(Kernel.cstring(pEntry->Desc, MaxDescsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->DescEx, MaxDescexsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->IntoneEffect, MaxEffectsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->IntoneSound, MaxSoundsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->SenderEffect, MaxEffectsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->SenderSound, MaxSoundsize) + " ");
                    builder.Append(pEntry->TargetDelay + " ");
                    builder.Append(Kernel.cstring(pEntry->TargetEffect, MaxEffectsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->TargetSound, MaxSoundsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->GroundEffect, MaxEffectsize) + " ");
                    builder.Append(Kernel.cstring(pEntry->TraceEffect, MaxEffectsize) + " ");
                    builder.Append(pEntry->ScreenRepresent + " ");
                    builder.Append(pEntry->CanBeusedInMarket + " ");
                    builder.Append(pEntry->TargetWoundDelay);
                    stream.WriteLine(builder.ToString());
                }
            }
        }

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public int GenUniqueId(Entry entry) => (int)(entry.MagicType * 10 + entry.Level);

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public int GenUniqueId(int magicType, uint level) => (int)(magicType * 10 + level);

        /// <summary>
        /// Get the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Get an array containing the keys of the dictionary.
        /// </summary>
        public int[] Keys
        { get { 
            lock (_entries)
            {
                var keys = new int[_entries.Count];
                _entries.Keys.CopyTo(keys, 0);
                return keys;
            }
        } }

        /// <summary>
        /// Get an array containing the values of the dictionary.
        /// </summary>
        public Entry[] Values
        { get {
            lock (_entries)
            {
                var values = new Entry[_entries.Count];

                var i = 0;
                foreach (var ptr in _entries.Values)
                {
                    fixed (Entry* pEntry = &values[i])
                        Kernel.memcpy(pEntry, (Entry*)ptr, sizeof(Entry));
                    i++;
                }
                return values;
            }
        } }

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
        /// Get the information of the specified type for the specified level.
        /// To get a valid string for some fields, use Kernel.cstring function.
        /// </summary>
        public bool TryGetValue(int uniqId, out Entry entry)
        {
            entry = new Entry();
            lock (_entries)
            {
                if (!_entries.ContainsKey(uniqId)) return false;
                fixed (Entry* pEntry = &entry)
                    Kernel.memcpy(pEntry, (Entry*)_entries[uniqId], sizeof(Entry));
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
                if (_entries.ContainsKey((int) (entry.MagicType*10 + entry.Level))) return false;
                var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));
                Kernel.memcpy(pEntry, &entry, sizeof(Entry));

                _entries.Add((int)(pEntry->MagicType * 10 + pEntry->Level), (IntPtr)pEntry);
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
                Kernel.memcpy((Entry*)_entries[uniqId], &entry, sizeof(Entry));
                return true;
            }
        }
    }
}

// * ************************************************************
// * * END:                                        magictype.cs *
// * ************************************************************