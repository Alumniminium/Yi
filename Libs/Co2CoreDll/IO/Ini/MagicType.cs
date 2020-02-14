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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.IO
{
    /// <summary>
    /// MagicType
    /// </summary>
    public unsafe class MagicType
    {
        public const Int32 MAX_NAMESIZE = 0x10;
        public const Int32 MAX_DESCSIZE = 0x40;
        public const Int32 MAX_DESCEXSIZE = 0x100;
        public const Int32 MAX_EFFECTSIZE = 0x40;
        public const Int32 MAX_SOUNDSIZE = 0x104;

        //Constants for magic's action sort field.
        public const UInt32 MAGIC_SORT_ATTACK_SINGLE_HP			= 1;
        public const UInt32 MAGIC_SORT_RECOVER_SINGLE_HP		    = 2;
        public const UInt32 MAGIC_SORT_ATTACK_CROSS_HP			= 3;
        public const UInt32 MAGIC_SORT_ATTACK_SECTOR_HP			= 4;
        public const UInt32 MAGIC_SORT_ATTACK_ROUND_HP			= 5;
        public const UInt32 MAGIC_SORT_ATTACK_SINGLE_STATUS		= 6;
        public const UInt32 MAGIC_SORT_RECOVER_SINGLE_STATUS	    = 7;
        public const UInt32 MAGIC_SORT_SQUARE					= 8;
        public const UInt32 MAGIC_SORT_JUMPATTACK				= 9;
        public const UInt32 MAGIC_SORT_RANDOMTRANS				= 10;
        public const UInt32 MAGIC_SORT_DISPATCHXP				= 11;
        public const UInt32 MAGIC_SORT_COLLIDE					= 12;
        public const UInt32 MAGIC_SORT_SERIALCUT				    = 13;
        public const UInt32 MAGIC_SORT_LINE						= 14;
        public const UInt32 MAGIC_SORT_ATKRANGE					= 15;
        public const UInt32 MAGIC_SORT_ATKSTATUS				    = 16;
        public const UInt32 MAGIC_SORT_CALL_TEAMMEMBER			= 17;
        public const UInt32 MAGIC_SORT_RECORDTRANSSPELL			= 18;	// record map position to trans spell.
        public const UInt32 MAGIC_SORT_TRANSFORM				    = 19;
        public const UInt32 MAGIC_SORT_ADDMANA					= 20;	// support self target only.
        public const UInt32 MAGIC_SORT_LAYTRAP					= 21;
        public const UInt32 MAGIC_SORT_DANCE					    = 22;
        public const UInt32 MAGIC_SORT_CALLPET			        = 23;
        public const UInt32 MAGIC_SORT_VAMPIRE			        = 24;	//power is percent award. use for call pet
        public const UInt32 MAGIC_SORT_INSTEAD			        = 25;	//use for call pet
        public const UInt32 MAGIC_SORT_DECLIFE			        = 26;
        public const UInt32 MAGIC_SORT_GROUNDSTING		        = 27;
        public const UInt32 MAGIC_SORT_REBORN			        = 28;
        public const UInt32 MAGIC_SORT_TEAM_MAGIC		        = 29;
        public const UInt32 MAGIC_SORT_BOMB_LOCKALL		        = 30;
        public const UInt32 MAGIC_SORT_SORB_SOUL			        = 31;
        public const UInt32 MAGIC_SORT_STEAL				        = 32;
        public const UInt32 MAGIC_SORT_LINE_PENETRABLE	        = 33;
        public const UInt32 MAGIC_SORT_BLAST_THUNDER			    = 34;
        public const UInt32 MAGIC_SORT_MULTI_ATTACHSTATUS	    = 35;
        public const UInt32 MAGIC_SORT_MULTI_DETACHSTATUS	    = 36;
        public const UInt32 MAGIC_SORT_MULTI_CURE			    = 37;
        public const UInt32 MAGIC_SORT_STEAL_MONEY			    = 38;
        public const UInt32 MAGIC_SORT_KO					    = 39;
        public const UInt32 MAGIC_SORT_ESCAPE				    = 40;
        public const UInt32 MAGIC_SORT_FLASH_ATTACK			    = 41;
        public const UInt32 MAGIC_SORT_ATTRACK_MONSTER		    = 42;

        //Constants for magic's target field.
        public const UInt32 MAGIC_TARGET_SELF		    = 0x00000001;
        public const UInt32 MAGIC_TARGET_NONE		    = 0x00000002;
        public const UInt32 MAGIC_TARGET_TERRAIN         = 0x00000004;
        public const UInt32 MAGIC_PASSIVE			    = 0x00000008;
        public const UInt32 MAGIC_TARGET_BODY		    = 0x00000010;

        //Constants for magic's xp field.
        public const UInt32 TYPE_MAGIC		= 0;
        public const UInt32 TYPE_XPSKILL	    = 1;
        public const UInt32 TYPE_KONGFU      = 2;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public Int32 Amount;
            public fixed Int32 UIDs[1]; //Type * 10 + Level
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public Int32 MagicType;
            public UInt32 ActionSort;
            public fixed Byte Name[MAX_NAMESIZE];
            public UInt32 Crime;
            public UInt32 Ground;
            public UInt32 Multi;
            public UInt32 Target;
            public UInt32 Level;
            public UInt32 MpCost;
            public UInt32 Power;
            public UInt32 IntoneDuration;
            public UInt32 Success;
            public UInt32 Time;
            public UInt32 Range; //X= %100, y= %10000 - x*100
            public UInt32 Distance;
            public UInt32 Status;
            public UInt32 ProfessionalRequired;
            public UInt32 ExpRequired;
            public UInt32 LevelRequired;
            public UInt32 Xp;
            public UInt32 WeaponSubType;
            public UInt32 ActiveTime;
            public UInt32 AutoActive;
            public UInt32 FloorAttribute;
            public UInt32 AutoLearn;
            public UInt32 LearnLevel;
            public UInt32 DropWeapon;
            public UInt32 UsePP;
            public UInt32 WeaponHit;
            public Int32 UseItem;
            public Int32 NextMagic;
            public UInt32 Delay;
            public UInt32 UseItemNum;
            public UInt32 SenderAction;
            public fixed Byte Desc[MAX_DESCSIZE];
            public fixed Byte DescEx[MAX_DESCEXSIZE];
            public fixed Byte IntoneEffect[MAX_EFFECTSIZE];
            public fixed Byte IntoneSound[MAX_SOUNDSIZE];
            public fixed Byte SenderEffect[MAX_EFFECTSIZE];
            public fixed Byte SenderSound[MAX_SOUNDSIZE];
            public UInt32 TargetDelay;
            public fixed Byte TargetEffect[MAX_EFFECTSIZE];
            public fixed Byte TargetSound[MAX_SOUNDSIZE];
            public fixed Byte GroundEffect[MAX_EFFECTSIZE];
            public fixed Byte TraceEffect[MAX_EFFECTSIZE];
            public UInt32 ScreenRepresent;
            public UInt32 CanBeusedInMarket;
            public UInt32 TargetWoundDelay;
        };

        private Dictionary<Int32, IntPtr> Entries = null;

        /// <summary>
        /// Create a new MagicType instance to handle the TQ's MagicType file.
        /// </summary>
        public MagicType()
        {
            this.Entries = new Dictionary<Int32, IntPtr>();
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
            if (Entries != null)
            {
                lock (Entries)
                {
                    foreach (var Ptr in Entries.Values)
                        Kernel.free((Entry*)Ptr);
                }
                Entries.Clear();
            }
        }

        /// <summary>
        /// Load the specified magictype file (in binary format) into the dictionary.
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
                    var Length = (Amount + 1) * sizeof(Int32);

                    var pHeader = (Header*)Kernel.malloc(Length);
                    Stream.Seek(0, SeekOrigin.Begin);
                    Stream.Read(Buffer, 0, Length);
                    Kernel.memcpy((Byte*)pHeader, Buffer, Length);

                    Entries = new Dictionary<Int32, IntPtr>(Amount);
                    for (var i = 0; i < Amount; i++)
                    {
                        var pEntry = (Entry*)Kernel.malloc(sizeof(Entry));

                        Stream.Read(Buffer, 0, sizeof(Entry));
                        Kernel.memcpy((Byte*)pEntry, Buffer, sizeof(Entry));

                        if (!Entries.ContainsKey(pHeader->UIDs[i]))
                            Entries.Add(pHeader->UIDs[i], (IntPtr)pEntry);
                    }
                    Kernel.free(pHeader);
                }
            }
        }

        /// <summary>
        /// Load the specified magictype file (in plain format) into the dictionary.
        /// </summary>
        public void LoadFromTxt(String Path)
        {
            Clear();

            lock (Entries)
            {
                using (var Stream = new StreamReader(Path, Encoding.GetEncoding("Windows-1252")))
                {
                    Entries = new Dictionary<Int32, IntPtr>();

                    String Line = null;
                    var LineC = 0;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        LineC++;

                        var Parts = Line.Split(' ');
                        var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));
                        Byte[] Buffer = null;

                        try
                        {
                            pEntry->MagicType = Int32.Parse(Parts[0]);
                            pEntry->ActionSort = UInt32.Parse(Parts[1]);
                            Buffer = Encoding.Default.GetBytes(Parts[2]);
                            Kernel.memcpy(pEntry->Name, Buffer, Buffer.Length);
                            pEntry->Crime = UInt32.Parse(Parts[3]);
                            pEntry->Ground = UInt32.Parse(Parts[4]);
                            pEntry->Multi = UInt32.Parse(Parts[5]);
                            pEntry->Target = UInt32.Parse(Parts[6]);
                            pEntry->Level = UInt32.Parse(Parts[7]);
                            pEntry->MpCost = UInt32.Parse(Parts[8]);
                            pEntry->Power = UInt32.Parse(Parts[9]);
                            pEntry->IntoneDuration = UInt32.Parse(Parts[10]);
                            pEntry->Success = UInt32.Parse(Parts[11]);
                            pEntry->Time = UInt32.Parse(Parts[12]);
                            pEntry->Range = UInt32.Parse(Parts[13]);
                            pEntry->Distance = UInt32.Parse(Parts[14]);
                            pEntry->Status = UInt32.Parse(Parts[15]);
                            pEntry->ProfessionalRequired = UInt32.Parse(Parts[16]);
                            pEntry->ExpRequired = UInt32.Parse(Parts[17]);
                            pEntry->LevelRequired = UInt32.Parse(Parts[18]);
                            pEntry->Xp = UInt32.Parse(Parts[19]);
                            pEntry->WeaponSubType = UInt32.Parse(Parts[20]);
                            pEntry->ActiveTime = UInt32.Parse(Parts[21]);
                            pEntry->AutoActive = UInt32.Parse(Parts[22]);
                            pEntry->FloorAttribute = UInt32.Parse(Parts[23]);
                            pEntry->AutoLearn = UInt32.Parse(Parts[24]);
                            pEntry->LearnLevel = UInt32.Parse(Parts[25]);
                            pEntry->DropWeapon = UInt32.Parse(Parts[26]);
                            pEntry->UsePP = UInt32.Parse(Parts[27]);
                            pEntry->WeaponHit = UInt32.Parse(Parts[28]);
                            pEntry->UseItem = Int32.Parse(Parts[29]);
                            pEntry->NextMagic = Int32.Parse(Parts[30]);
                            pEntry->Delay = UInt32.Parse(Parts[31]);
                            pEntry->UseItemNum = UInt32.Parse(Parts[32]);
                            pEntry->SenderAction = UInt32.Parse(Parts[33]);
                            Buffer = Encoding.Default.GetBytes(Parts[34]);
                            Kernel.memcpy(pEntry->Desc, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[35]);
                            Kernel.memcpy(pEntry->DescEx, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[36]);
                            Kernel.memcpy(pEntry->IntoneEffect, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[37]);
                            Kernel.memcpy(pEntry->IntoneSound, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[38]);
                            Kernel.memcpy(pEntry->SenderEffect, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[39]);
                            Kernel.memcpy(pEntry->SenderSound, Buffer, Buffer.Length);
                            pEntry->TargetDelay = UInt32.Parse(Parts[40]);
                            Buffer = Encoding.Default.GetBytes(Parts[41]);
                            Kernel.memcpy(pEntry->TargetEffect, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[42]);
                            Kernel.memcpy(pEntry->TargetSound, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[43]);
                            Kernel.memcpy(pEntry->GroundEffect, Buffer, Buffer.Length);
                            Buffer = Encoding.Default.GetBytes(Parts[44]);
                            Kernel.memcpy(pEntry->TraceEffect, Buffer, Buffer.Length);
                            pEntry->ScreenRepresent = UInt32.Parse(Parts[45]);
                            pEntry->CanBeusedInMarket = UInt32.Parse(Parts[46]);
                            pEntry->TargetWoundDelay = UInt32.Parse(Parts[47]);

                            if (!Entries.ContainsKey((Int32)((pEntry->MagicType * 10) + pEntry->Level)))
                                Entries.Add((Int32)((pEntry->MagicType * 10) + pEntry->Level), (IntPtr)pEntry);
                        }
                        catch (Exception Exc)
                        { 
                            Console.WriteLine("Error at line {0}.\n{1}", LineC, Exc);
                            Kernel.free(pEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified magictype file (in binary format).
        /// </summary>
        public void SaveToDat(String Path)
        {
            using (var Stream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var Buffer = new Byte[Kernel.MAX_BUFFER_SIZE];
                var Pointers = new IntPtr[0];

                lock (Entries)
                {
                    Pointers = new IntPtr[Entries.Count];
                    Entries.Values.CopyTo(Pointers, 0);
                }

                var Length = (Pointers.Length + 1) * sizeof(Int32);

                var pHeader = (Header*)Kernel.malloc(Length);
                pHeader->Amount = Pointers.Length;
                for (var i = 0; i < Pointers.Length; i++)
                    pHeader->UIDs[i] = (Int32)((((Entry*)Pointers[i])->MagicType * 10) + ((Entry*)Pointers[i])->Level);

                Kernel.memcpy(Buffer, pHeader, Length);
                Stream.Write(Buffer, 0, Length);

                for (var i = 0; i < Pointers.Length; i++)
                {
                    Kernel.memcpy(Buffer, (Entry*)Pointers[i], sizeof(Entry));
                    Stream.Write(Buffer, 0, sizeof(Entry));
                }
                Kernel.free(pHeader);
            }
        }

        /// <summary>
        /// Save all the dictionary to the specified magictype file (in plain format).
        /// </summary>
        public void SaveToTxt(String Path)
        {
            using (var Stream = new StreamWriter(Path, false, Encoding.GetEncoding("Windows-1252")))
            {
                var Pointers = new IntPtr[0];

                lock (Entries)
                {
                    Pointers = new IntPtr[Entries.Count];
                    Entries.Values.CopyTo(Pointers, 0);
                }

                for (var i = 0; i < Pointers.Length; i++)
                {
                    var pEntry = (Entry*)Pointers[i];

                    var Builder = new StringBuilder(Kernel.MAX_BUFFER_SIZE);
                    Builder.Append(pEntry->MagicType + " ");
                    Builder.Append(pEntry->ActionSort + " ");
                    Builder.Append(Kernel.cstring(pEntry->Name, MAX_NAMESIZE) + " ");
                    Builder.Append(pEntry->Crime + " ");
                    Builder.Append(pEntry->Ground + " ");
                    Builder.Append(pEntry->Multi + " ");
                    Builder.Append(pEntry->Target + " ");
                    Builder.Append(pEntry->Level + " ");
                    Builder.Append(pEntry->MpCost + " ");
                    Builder.Append(pEntry->Power + " ");
                    Builder.Append(pEntry->IntoneDuration + " ");
                    Builder.Append(pEntry->Success + " ");
                    Builder.Append(pEntry->Time + " ");
                    Builder.Append(pEntry->Range + " ");
                    Builder.Append(pEntry->Distance + " ");
                    Builder.Append(pEntry->Status + " ");
                    Builder.Append(pEntry->ProfessionalRequired + " ");
                    Builder.Append(pEntry->ExpRequired + " ");
                    Builder.Append(pEntry->LevelRequired + " ");
                    Builder.Append(pEntry->Xp + " ");
                    Builder.Append(pEntry->WeaponSubType + " ");
                    Builder.Append(pEntry->ActiveTime + " ");
                    Builder.Append(pEntry->AutoActive + " ");
                    Builder.Append(pEntry->FloorAttribute + " ");
                    Builder.Append(pEntry->AutoLearn + " ");
                    Builder.Append(pEntry->LearnLevel + " ");
                    Builder.Append(pEntry->DropWeapon + " ");
                    Builder.Append(pEntry->UsePP + " ");
                    Builder.Append(pEntry->WeaponHit + " ");
                    Builder.Append(pEntry->UseItem + " ");
                    Builder.Append(pEntry->NextMagic + " ");
                    Builder.Append(pEntry->Delay + " ");
                    Builder.Append(pEntry->UseItemNum + " ");
                    Builder.Append(pEntry->SenderAction + " ");
                    Builder.Append(Kernel.cstring(pEntry->Desc, MAX_DESCSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->DescEx, MAX_DESCEXSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->IntoneEffect, MAX_EFFECTSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->IntoneSound, MAX_SOUNDSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->SenderEffect, MAX_EFFECTSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->SenderSound, MAX_SOUNDSIZE) + " ");
                    Builder.Append(pEntry->TargetDelay + " ");
                    Builder.Append(Kernel.cstring(pEntry->TargetEffect, MAX_EFFECTSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->TargetSound, MAX_SOUNDSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->GroundEffect, MAX_EFFECTSIZE) + " ");
                    Builder.Append(Kernel.cstring(pEntry->TraceEffect, MAX_EFFECTSIZE) + " ");
                    Builder.Append(pEntry->ScreenRepresent + " ");
                    Builder.Append(pEntry->CanBeusedInMarket + " ");
                    Builder.Append(pEntry->TargetWoundDelay);
                    Stream.WriteLine(Builder.ToString());
                }
            }
        }

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public Int32 GenUID(Entry Entry) { return (Int32)((Entry.MagicType * 10) + Entry.Level); }

        /// <summary>
        /// Generate the UniqId of the specified magic.
        /// </summary>
        public Int32 GenUID(Int32 MagicType, UInt32 Level) { return (Int32)((MagicType * 10) + Level); }

        /// <summary>
        /// Get the number of key/value pairs contained in the dictionary.
        /// </summary>
        public Int32 Count { get { return Entries.Count; } }

        /// <summary>
        /// Get an array containing the keys of the dictionary.
        /// </summary>
        public Int32[] Keys
        { get { 
            lock (Entries)
            {
                var Keys = new Int32[Entries.Count];
                Entries.Keys.CopyTo(Keys, 0);
                return Keys;
            }
        } }

        /// <summary>
        /// Get an array containing the values of the dictionary.
        /// </summary>
        public Entry[] Values
        { get {
            lock (Entries)
            {
                var Values = new Entry[Entries.Count];

                var i = 0;
                foreach (var Ptr in Entries.Values)
                {
                    fixed (Entry* pEntry = &Values[i])
                        Kernel.memcpy(pEntry, (Entry*)Ptr, sizeof(Entry));
                    i++;
                }
                return Values;
            }
        } }

        /// <summary>
        /// Determine whether the dictionary contains the specified key.
        /// </summary>
        public Boolean ContainsKey(Int32 UniqId)
        {
            lock (Entries)
            {
                if (Entries.ContainsKey(UniqId))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get the information of the specified type for the specified level.
        /// To get a valid string for some fields, use Kernel.cstring function.
        /// </summary>
        public Boolean TryGetValue(Int32 UniqId, out Entry Entry)
        {
            Entry = new Entry();
            lock (Entries)
            {
                if (Entries.ContainsKey(UniqId))
                {
                    fixed (Entry* pEntry = &Entry)
                        Kernel.memcpy(pEntry, (Entry*)Entries[UniqId], sizeof(Entry));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add the magic's information in the dictionary.
        /// It can be used to create an editor or an temp magic.
        /// </summary>
        public Boolean Add(Entry Entry)
        {
            lock (Entries)
            {
                if (!Entries.ContainsKey((Int32)((Entry.MagicType * 10) + Entry.Level)))
                {
                    var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));
                    Kernel.memcpy(pEntry, &Entry, sizeof(Entry));

                    Entries.Add((Int32)((pEntry->MagicType * 10) + pEntry->Level), (IntPtr)pEntry);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete the magic's information in the dictionary.
        /// </summary>
        public Boolean Remove(Int32 UniqId)
        {
            lock (Entries)
            {
                if (Entries.ContainsKey(UniqId))
                {
                    Entries.Remove(UniqId);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the magic's information in the dictionary.
        /// </summary>
        public Boolean Update(Int32 UniqId, Entry Entry)
        {
            lock (Entries)
            {
                if (Entries.ContainsKey(UniqId))
                {
                    Kernel.memcpy((Entry*)Entries[UniqId], &Entry, sizeof(Entry));
                    return true;
                }
            }
            return false;
        }
    }
}

// * ************************************************************
// * * END:                                        magictype.cs *
// * ************************************************************