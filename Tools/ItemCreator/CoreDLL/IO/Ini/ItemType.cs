// * ************************************************************
// * * START:                                       itemtype.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * ItemType class for the library.
// * itemtype.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (January 22th, 2012)
// * Copyright (C) 2012 CptSky
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
    /// ItemType
    /// </summary>
    public unsafe class ItemType
    {
        public const int MaxNamesize = 16;
        public const int MaxDescsize = 128;

        //Item monopoly bit field
        public const int ItemMonopolyMask = 0x01;
        public const int ItemStorageMask = 0x02;
        public const int ItemDropHintMask = 0x04;
        public const int ItemSellHintMask = 0x08;
        public const int ItemNeverDropWhenDeadMask = 0x10;
        public const int ItemSellDisableMask = 0x20;

        //Item status bit field
        public const int ItemStatusNone = 0;
        public const int ItemStatusNotIdent = 1;
        public const int ItemStatusCannotRepair = 2;
        public const int ItemStatusNeverDamage = 4;
        public const int ItemStatusMagicAdd = 8;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Entry
        {
            public int ID;
            public fixed byte Name[MaxNamesize];
            public byte RequiredProfession;
            public byte RequiredWeaponSkill;
            public byte RequiredLevel;
            public byte RequiredSex;
            public ushort RequiredForce;
            public ushort RequiredSpeed;
            public ushort RequiredHealth;
            public ushort RequiredSoul;
            public byte Monopoly;
            public ushort Weight;
            public uint Price;
            public int Action; //cq_action
            public ushort MaxAttack;
            public ushort MinAttack;
            public short Defense;
            public short Dexterity;
            public short Dodge;
            public short Life;
            public short Mana;
            public ushort Amount;
            public ushort AmountLimit; // x / 100
            public byte Status;
            public byte Gem1;
            public byte Gem2;
            public byte Magic1;
            public byte Magic2;
            public byte Magic3;
            public ushort MagicAttack;
            public ushort MagicDefence;
            public ushort Range;
            public ushort AttackSpeed;
            public byte FrayMode; //Good name?
            public byte RepairMode; //Good name?
            public byte TypeMask; //Good name?
            public uint ConquerPoints; //Good name?
            public fixed byte Type[MaxNamesize]; //Good name?
            public fixed byte Desc[MaxDescsize];
        }

        public Dictionary<int, Entry> Items;
        
        public ItemType()
        {
            Items = new Dictionary<int, Entry>();
        }
        
        public void LoadFromTxt(string path)
        {
            Items.Clear();

            lock (Items)
            {
                using (var stream = new StreamReader(path, Encoding.GetEncoding("Windows-1252")))
                {
                    Items = new Dictionary<int, Entry>();

                    string line;
                    var lineC = 0;
                    while ((line = stream.ReadLine()) != null)
                    {
                        if (lineC == 0 || string.IsNullOrEmpty(line)) //Amount = X
                        {
                            lineC++;
                            continue;
                        }
                        lineC++;

                        var parts = line.Split(' ');
                        var pEntry = new Entry();
                        
                        //var pEntry = (Entry*)Kernel.calloc(sizeof(Entry));

                        try
                        {
                            var buffer = new byte[Kernel.MAX_BUFFER_SIZE];
                            fixed (byte* pStr = buffer)
                            {

                                pEntry.ID = int.Parse(parts[0]);

                                parts[1].ToPointer(pStr);
                                Kernel.memcpy(pEntry.Name, pStr, Math.Min(MaxNamesize - 1, Kernel.strlen(pStr)));

                                pEntry.RequiredProfession = byte.Parse(parts[2]);
                                pEntry.RequiredWeaponSkill = byte.Parse(parts[3]);
                                pEntry.RequiredLevel = byte.Parse(parts[4]);
                                pEntry.RequiredSex = byte.Parse(parts[5]);
                                pEntry.RequiredForce = ushort.Parse(parts[6]);
                                pEntry.RequiredSpeed = ushort.Parse(parts[7]);
                                pEntry.RequiredHealth = ushort.Parse(parts[8]);
                                pEntry.RequiredSoul = ushort.Parse(parts[9]);
                                pEntry.Monopoly = byte.Parse(parts[10]);
                                pEntry.Weight = ushort.Parse(parts[11]);
                                pEntry.Price = uint.Parse(parts[12]);
                                pEntry.Action = int.Parse(parts[13]);
                                pEntry.MaxAttack = ushort.Parse(parts[14]);
                                pEntry.MinAttack = ushort.Parse(parts[15]);
                                pEntry.Defense = short.Parse(parts[16]);
                                pEntry.Dexterity = short.Parse(parts[17]);
                                pEntry.Dodge = short.Parse(parts[18]);
                                pEntry.Life = short.Parse(parts[19]);
                                pEntry.Mana = short.Parse(parts[20]);
                                pEntry.Amount = ushort.Parse(parts[21]);
                                pEntry.AmountLimit = ushort.Parse(parts[22]);
                                pEntry.Status = byte.Parse(parts[23]);
                                pEntry.Gem1 = byte.Parse(parts[24]);
                                pEntry.Gem2 = byte.Parse(parts[25]);
                                pEntry.Magic1 = byte.Parse(parts[26]);
                                pEntry.Magic2 = byte.Parse(parts[27]);
                                pEntry.Magic3 = byte.Parse(parts[28]);
                                pEntry.MagicAttack = ushort.Parse(parts[29]);
                                pEntry.MagicDefence = ushort.Parse(parts[30]);
                                pEntry.Range = ushort.Parse(parts[31]);
                                pEntry.AttackSpeed = ushort.Parse(parts[32]);
                                pEntry.FrayMode = byte.Parse(parts[33]);
                                pEntry.RepairMode = byte.Parse(parts[34]);
                                pEntry.TypeMask = byte.Parse(parts[35]);
                                pEntry.ConquerPoints = uint.Parse(parts[36]);

                                parts[37].ToPointer(pStr);
                                Kernel.memcpy(pEntry.Type, pStr, Math.Min(MaxNamesize - 1, Kernel.strlen(pStr)));
                                parts[38].ToPointer(pStr);
                                Kernel.memcpy(pEntry.Type, pStr, Math.Min(MaxDescsize - 1, Kernel.strlen(pStr)));

                                if (!Items.ContainsKey(pEntry.ID))
                                    Items.Add(pEntry.ID, pEntry);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Error at line {0}.\n{1}", lineC, exc);
                            //Kernel.free(pEntry);
                        }
                    }
                }
            }
        }
        
        public void SaveToTxt(string path)
        {
            using (var stream = new StreamWriter(path, false, Encoding.GetEncoding("Windows-1252")))
            {
                stream.WriteLine("Amount={0}", Items.Count);
                foreach (var entry in Items.Values)
                {
                    var pEntry = entry;
                    var builder = new StringBuilder(Kernel.MAX_BUFFER_SIZE);
                    builder.Append(pEntry.ID + " ");
                    builder.Append(Kernel.cstring(pEntry.Name, MaxNamesize) + " ");
                    builder.Append(pEntry.RequiredProfession + " ");
                    builder.Append(pEntry.RequiredWeaponSkill + " ");
                    builder.Append(pEntry.RequiredLevel + " ");
                    builder.Append(pEntry.RequiredSex + " ");
                    builder.Append(pEntry.RequiredForce + " ");
                    builder.Append(pEntry.RequiredSpeed + " ");
                    builder.Append(pEntry.RequiredHealth + " ");
                    builder.Append(pEntry.RequiredSoul + " ");
                    builder.Append(pEntry.Monopoly + " ");
                    builder.Append(pEntry.Weight + " ");
                    builder.Append(pEntry.Price + " ");
                    builder.Append(pEntry.Action + " ");
                    builder.Append(pEntry.MaxAttack + " ");
                    builder.Append(pEntry.MinAttack + " ");
                    builder.Append(pEntry.Defense + " ");
                    builder.Append(pEntry.Dexterity + " ");
                    builder.Append(pEntry.Dodge + " ");
                    builder.Append(pEntry.Life + " ");
                    builder.Append(pEntry.Mana + " ");
                    builder.Append(pEntry.Amount + " ");
                    builder.Append(pEntry.AmountLimit + " ");
                    builder.Append(pEntry.Status + " ");
                    builder.Append(pEntry.Gem1 + " ");
                    builder.Append(pEntry.Gem2 + " ");
                    builder.Append(pEntry.Magic1 + " ");
                    builder.Append(pEntry.Magic2 + " ");
                    builder.Append(pEntry.Magic3 + " ");
                    builder.Append(pEntry.MagicAttack + " ");
                    builder.Append(pEntry.MagicDefence + " ");
                    builder.Append(pEntry.Range + " ");
                    builder.Append(pEntry.AttackSpeed + " ");
                    builder.Append(pEntry.FrayMode + " ");
                    builder.Append(pEntry.RepairMode + " ");
                    builder.Append(pEntry.TypeMask + " ");
                    builder.Append(pEntry.ConquerPoints + " ");
                    builder.Append(Kernel.cstring(pEntry.Type, MaxNamesize) + " ");
                    builder.Append(Kernel.cstring(pEntry.Desc, MaxDescsize));
                    stream.WriteLine(builder.ToString());
                }
            }
        }
    }
}

// * ************************************************************
// * * END:                                         itemtype.cs *
// * ************************************************************