using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Yi.Core;
using Yi.Core.Calculations;
using Yi.Core.Enums;
using Yi.Core.Helpers;
using Yi.Items;
using Yi.Network.Packets.Conquer;
using Yi.Structures;
using Yi.Structures.SelfContainedSystems;
using Yi.World;

namespace Yi.Entities
{
    public class YiObject
    {
        public virtual string Name { get; set; }
        public bool Alive => CurrentHp > 0;
        public uint UniqueId;
        public long Look;
        public ushort MapId;
        public short X;
        public short Y;
        public Direction Direction;
        public virtual byte Level { get; set; }
        public virtual int CurrentHp { get; set; }
        public virtual ushort MaximumHp { get; set; }
        public ushort Dexterity;
        public float Dodge;
        public int Defense;
        public int MagicDefense;
        public Emote Emote;
        public int AccuracyEndTime;
        public int StarOfAccuracyEndTime;
        public int ShieldEndTime;
        public int MagicShieldEndTime;
        public int StigmaEndTime;
        public int InvisibilityEndTime;
        public int AzureShieldEndTime;
        public int TransformEndTime;
        public int SupermanEndTime;
        public int CycloneEndTime;
        public int FlyEndTime;
        public float DefenseBonus;
        public float DexterityBonus;
        public StatusEffect StatusEffects;
        public bool HasFlag(StatusEffect flag) => (StatusEffects & flag) != 0;
        public Trade Trade;
        public bool Online;
        public ushort Aggro;
        public virtual ushort Agility { get; set; }
        public virtual uint Experience { get; set; }
        public uint Hair;
        public bool Mining;
        public string Partner;
        public ushort PkPoints;
        public virtual ushort Vitality { get; set; }
        public uint WarehouseMoney;
        public GuildRanks GuildRank;
        public uint GuildDonation;
        public float Bless;
        public virtual byte Stamina { get; set; }
        public PkMode PkMode;
        public Dictionary<int, Prof> Profs;
        public Dictionary<uint, Skill> Skills;
        public ConcurrentDictionary<uint, YiObject> Enemies;
        public ConcurrentDictionary<uint, YiObject> Friends;
        public virtual byte Class { get; set; }
        public virtual uint Money { get; set; }
        public virtual bool Reborn { get; set; }
        public virtual ushort Spirit { get; set; }
        public virtual ushort Statpoints { get; set; }
        public virtual ushort Strength { get; set; }
        public int VirturePoints;
        public int DisKo;
        public float ExpBonus;
        public float WeaponExpBonus;
        public float MagicExpBonus;
        public Guild Guild;
        public bool IsWaterTaoist() => Class > 130 && Class < 136;
        public bool IsInJail() => MapId == 6000;
        public virtual ushort CurrentMp { get; set; }
        public virtual ushort MaximumMp { get; set; }
        public int XpTime;
        public int Xp;
        public int XpKills;
        public int MaximumPhsyicalAttack;
        public int MinimumPhsyicalAttack;
        public int MagicAttack;
        public bool IsInBattle;
        private int _attackRange;
        public float AttackBonus;
        public float AttackSpeedBonus;
        public int AttackRange
        {
            get
            {
                Item weapon;
                if (Equipment != null && Equipment.TryGetValue(MsgItemPosition.MainHand, out weapon))
                    return weapon.Range;
                return Math.Max(_attackRange, 1);
            }
            set { _attackRange = value; }
        }

        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        public short MagicType;
        public short MagicHitRate;
        public uint AttackSpeed;
        public Lazy<ConcurrentDictionary<uint, YiObject>> PissedAt;
        public Lazy<List<Point>> Waypoints;
        public YiObject CurrentTarget;
        public Skill CurrentSkill;

        public virtual void KilledFrom(YiObject attacker)
        {
            if (HasFlag(StatusEffect.SuperMan) || HasFlag(StatusEffect.Cyclone))
                (attacker as Player)?.Send(MsgInteract.Create(attacker, this, MsgInteractType.Death, 0xFFFF * attacker.XpKills));
            else
                ScreenSystem.Send(this, MsgInteract.Die(attacker, this), true);

            var human = attacker;
            if (human != null)
                human.PkPoints++;
        }
        public virtual void Respawn() => EntityLogic.Recalculate(this);
        public void UseItem(Item item)
        {
        }
        public void RemoveDura(DuraType attack)
        {
        }
        public void RemoveDura(MsgItemPosition boots)
        {
        }
        public void AddWeaponProf(MsgItemPosition position, uint exp)
        {
            var item = GetEquip(position);
            if (item != null)
            {
                Prof prof;
                if (Profs.TryGetValue((int)(item.ItemId / 1000), out prof))
                    prof.Experience += exp;
            }
        }
        public void AddArmorProf(MsgItemPosition position, uint exp)
        {
            var item = GetEquip(position);
            if (item != null)
            {
                Prof prof;
                if (Profs.TryGetValue((int)(item.ItemId / 1000), out prof))
                    prof.Experience += exp;
            }
        }
        public bool ActivatePassiveSkill(ref int damage, out Skill? s)
        {
            foreach (var skill in from skill in Skills.Values from item in Equipment.Values where item.ItemId / 1000 == skill.Info.WeaponSubType where YiCore.Random.Next(0, 100) >= skill.Info.Success select skill)
            {
                damage += (int)skill.Info.Power;
                //Screen.Send(LegacyPackets.MsgMagicAttack(this, skill, Position.GetTargets(this,skill.Info.Type, damage)),true);
                s = skill;
                return true;
            }
            s = null;
            return false;
        }
        
        public Item GetEquip(MsgItemPosition position)
        {
            Item found;
            Equipment.TryGetValue(position, out found);
            return found;
        }
        public virtual void GetMessage(string @from, string to, string message, MsgTextType type)
        {
        }
        public void AddTransform(long transformId) => Look = (uint)(transformId * 10000000L + Look % 10000000L);
        public void DelTransform() => Look = Look % 10000000;
        public void DropEquipment()
        {
        }
        public void AddStatusEffect(StatusEffect flag)
        {
            StatusEffects |= flag;
            ScreenSystem.Send(this, MsgUpdate.Create(this, (ulong)StatusEffects, MsgUpdateType.StatusEffect), true);
        }
        public void RemoveStatusEffect(StatusEffect flag)
        {
            StatusEffects &= ~flag;
            ScreenSystem.Send(this, MsgUpdate.Create(this, (ulong)StatusEffects, MsgUpdateType.StatusEffect), true);
        }
        public void Teleport(short x, short y, ushort map)
        {
            GameWorld.Maps[MapId].Leave(this);
            MapId = map;
            X = x;
            Y = y;
            GameWorld.Maps[MapId].Enter(this);
        }
        public void GetHit(YiObject attacker, int damage)
        {
            if (!Alive)
                return;
            CurrentHp -= damage;
            if (CurrentHp <= 0)
                KilledFrom(attacker);
        }

        public bool IsGreen(YiObject mapObject)
        {
            return mapObject.Level - Level >= 3;
        }

        public bool IsWhite(YiObject mapObject)
        {
            return mapObject.Level - Level >= 0 && mapObject.Level - Level < 3;
        }

        public bool IsRed(YiObject mapObject)
        {
            return mapObject.Level - Level >= -4 && mapObject.Level - Level < 0;
        }

        public bool IsBlack(YiObject mapObject)
        {
            return mapObject.Level - Level < -4;
        }
    }
}
