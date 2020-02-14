using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using YiX.Calculations;
using YiX.Database.Squiggly.Models;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Packets.Conquer;
using YiX.Scheduler;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Entities
{
    [Serializable]
    public class YiObj
    {
        public virtual string Name { get; set; }
        public bool Alive => CurrentHp > 0 && !HasFlag(StatusEffect.Die);
        public int UniqueId { get; set; }
        public virtual uint Look { get; set; }
        public ushort MapId { get; set; }
        public Vector2 Location { get; set; }
        public virtual Direction Direction { get; set; }
        public virtual byte Level { get; set; }
        public virtual int CurrentHp { get; set; }
        public virtual ushort MaximumHp { get; set; }
        public ushort Dexterity { get; set; }
        public int Defense { get; set; }
        public int MagicDefense { get; set; }
        public Emote Emote { get; set; }
        public StatusEffect StatusEffects { get; set; }
        public bool HasFlag(StatusEffect flag) => (StatusEffects & flag) != 0;
        public int BoothId { get; set; }
        public virtual ushort Agility { get; set; }
        public virtual uint Experience { get; set; }
        public uint Hair { get; set; }
        public bool Mining { get; set; }
        public string Partner { get; set; }
        public virtual ushort PkPoints { get; set; }
        public virtual ushort Vitality { get; set; }
        public uint WarehouseMoney { get; set; }
        public int GuildDonation { get; set; }
        public float Bless { get; set; }
        public virtual byte Stamina { get; set; }
        public virtual byte Class { get; set; }
        public virtual int Money { get; set; }
        public virtual byte Reborn { get; set; }
        public virtual ushort Spirit { get; set; }
        public virtual ushort Statpoints { get; set; }
        public virtual ushort Strength { get; set; }
        public int VirturePoints { get; set; }
        public int DisKo { get; set; }
        public float ExpBonus { get; set; }
        public float WeaponExpBonus { get; set; }
        public float MagicExpBonus { get; set; }
        public bool IsWaterTaoist() => Class > 130 && Class < 136;
        public bool IsInJail()
        {
            if (GameWorld.Maps.TryGetValue(MapId, out var map))
                return map.Flags.HasFlag(MapFlags.Prison);
            return MapId == 6000;
        }

        public virtual ushort CurrentMp { get; set; }
        public virtual ushort MaximumMp { get; set; }
        public int Xp;
        private float _attackBonus;
        public int XpKills { get; set; }
        public int MaximumPhsyicalAttack { get; set; }
        public int MinimumPhsyicalAttack { get; set; }
        public int MagicAttack { get; set; }
        public bool IsInBattle { get; set; }
        private int _attackRange { get; set; }

        public float AttackBonus
        {
            get
            {
                if (BuffSystem.Entries.TryGetValue(UniqueId, out var buffs))
                {
                    foreach (var buff in buffs)
                        _attackBonus += buff.Value.PhysAtkMod;
                }
                return _attackBonus;
            }
        }

        public float AttackSpeedBonus { get; set; }
        public short MagicType { get; set; }
        public short MagicHitRate { get; set; }
        public int AttackSpeed { get; set; } = 1000;
        public int AttackRange
        {
            get
            {
                if (Equipment != null && Equipment.TryGetValue(MsgItemPosition.RightWeapon, out var weapon))
                    return Math.Max((int)weapon.Range, 2);
                return Math.Max(_attackRange, 1);
            }
            set => _attackRange = value;
        }
        [JsonIgnore]
        public Trade Trade { get; set; }
        public PkMode PkMode { get; set; }
        public Guild Guild { get; set; }
        public GuildRanks GuildRank { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        public Dictionary<int, Prof> Profs { get; set; }
        public Dictionary<SkillId, Skill> Skills { get; set; }
        public List<int> Enemies { get; set; }
        public List<int> Friends { get; set; }
        [JsonIgnore]
        public virtual YiObj CurrentTarget { get; set; }
        [JsonIgnore]
        public Skill CurrentSkill { get; set; }
        public int CurrentStorageId { get; set; }
        public cq_action CurrentAction { get; set; }

        protected YiObj()
        {
            Location = new Vector2(0, 0);
            Friends = new List<int>();
            Enemies = new List<int>();
        }
        public virtual void KilledFrom(YiObj attacker)
        {
            if (attacker is Player player)
            {
                if (player.AttackJob != null)
                    player.AttackJob.Cancelled = true;
            }

            BuffSystem.Clear(this);

            if (attacker.HasFlag(StatusEffect.SuperMan) || attacker.HasFlag(StatusEffect.Cyclone))
                (attacker as Player)?.Send(MsgInteract.Create(attacker, this, MsgInteractType.Death, 0xFFFF * attacker.XpKills));
            else
                ScreenSystem.Send(this, MsgInteract.Die(attacker, this), true);

            if (attacker is Player humanAttacker)
            {
                var killed = this as Player;
                if (killed != null && !killed.Enemies.Contains(humanAttacker.UniqueId))
                    (this as Player)?.Enemies.Add(humanAttacker.UniqueId);

                killed?.Send(MsgFriend.Create(humanAttacker, MsgFriendActionType.EnemyAdd, MsgFriendStatusType.Online));
            }

            if (attacker is Player attackerPlayer)
            {
                attackerPlayer.Xp++;
                if (attackerPlayer.HasFlag(StatusEffect.SuperMan))
                    BuffSystem.Entries[attackerPlayer.UniqueId][SkillId.Superman].AddTime(820);
                if (attacker.HasFlag(StatusEffect.Cyclone))
                    BuffSystem.Entries[attackerPlayer.UniqueId][SkillId.Cyclone].AddTime(820);

                attackerPlayer.CurrentKO++;

                attackerPlayer.Send(MsgInteract.Create(attackerPlayer.UniqueId, 0, 0, 0, MsgInteractType.Death, (int)(0xFFFF * attackerPlayer.CurrentKO)));
            }
        }
        public virtual void Respawn()
        {
            DelTransform();
            RemoveStatusEffect(StatusEffect.Die);
            EntityLogic.Recalculate(this);
        }
        public void AddWeaponProf(MsgItemPosition position, uint exp)
        {
            var item = GetEquip(position);
            if (item.Valid())
            {
                if (Profs.TryGetValue(item.ItemId / 1000, out var prof))
                    prof.Experience += exp;
                else
                    Profs.Add(item.ItemId / 1000, new Prof((ushort)(item.ItemId / 1000), 0, exp));
            }
        }
        public void AddArmorProf(MsgItemPosition position, uint exp)
        {
            var item = GetEquip(position);
            if (item.Valid())
            {
                if (Profs.TryGetValue(item.ItemId / 1000, out var prof))
                    prof.Experience += exp;
                else
                    Profs.Add(item.ItemId / 1000, new Prof((ushort)(item.ItemId / 1000), 0, exp));
            }
        }
        public bool ActivatePassiveSkill(YiObj attacker, ref int damage, out Skill? s)
        {
            foreach (var skill in from skill in Skills.Values from item in Equipment.Values where item.ItemId / 1000 == skill.Info.WeaponSubType where SafeRandom.Next(0, 100) >= skill.Info.Success select skill)
            {
                damage += AttackCalcs.GetDamage(attacker, attacker.CurrentTarget, MsgInteractType.Magic);
                damage += skill.Info.Power;
                s = skill;
                return true;
            }
            s = null;
            return false;
        }

        public Item GetEquip(MsgItemPosition position)
        {
            if (Equipment == null)
                return ItemFactory.CreateInvalidItem();

            Equipment.TryGetValue(position, out var found);
            return found;
        }

        public virtual void GetMessage(string from, string to, string message, MsgTextType type)
        {
        }

        public void AddTransform(long transformId) => Look = (uint)(transformId * 10000000L + Look % 10000000L);
        public void DelTransform() => Look = Look % 10000000;

        public void AddStatusEffect(StatusEffect flag)
        {
            StatusEffects |= flag;
            ScreenSystem.Send(this, MsgUpdate.Create(this, (long)StatusEffects, MsgUpdateType.StatusEffect), true);
            if (this is Player player)
                EntityLogic.Recalculate(player);
        }
        public void RemoveStatusEffect(StatusEffect flag)
        {
            StatusEffects &= ~flag;
            ScreenSystem.Send(this, MsgUpdate.Create(this, (long)StatusEffects, MsgUpdateType.StatusEffect), true);
            if (this is Player player)
                EntityLogic.Recalculate(player);
        }
        public void Teleport(ushort x, ushort y, ushort map)
        {
            try
            {
                GameWorld.Maps[MapId].Leave(this);
                MapId = map;
                Location.X = x;
                Location.Y = y;
                GameWorld.Maps[MapId].Enter(this);
                if (GameWorld.Maps[MapId].RespawnLocation == null)
                {
                    GetMessage("SYSTEM", Name, "THIS MAP HAS TO RESPAWN POINT! GO TO THE POSITION YOU WOULD RESPAWN AND TYPE 'setspawn' THEN SAVE THE DB!", MsgTextType.Center);
                }
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
                MapId = map;
                Location.X = x;
                Location.Y = y;
                (this as Player)?.Send(MsgAction.MapShowPacket(this));
            }
        }

        public void GetHit(YiObj attacker, int damage)
        {
            if (!Alive)
                return;

            CurrentHp -= damage;
            AddArmorProf(MsgItemPosition.Armor, (uint)damage);
            if (Look / 100 == 9 || this is Player)
            {
                if (attacker is Player pAttacker)
                {

                    if (!HasFlag(StatusEffect.Flashing))
                        attacker.AddStatusEffect(StatusEffect.Flashing);

                    if (pAttacker.FlashingJob == null)
                        pAttacker.FlashingJob = YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(30), () => pAttacker.RemoveStatusEffect(StatusEffect.Flashing));
                    else if (pAttacker.FlashingJob.ExecutionTime < DateTime.UtcNow)
                        pAttacker.FlashingJob.ExecutionTime = DateTime.UtcNow.AddSeconds(30);
                }
            }

            if (CurrentHp <= 0)
            {
                KilledFrom(attacker);
            }
        }

        public bool IsGreen(YiObj mapObj) => mapObj.Level - Level >= 3;

        public bool IsWhite(YiObj mapObj) => mapObj.Level - Level >= 0 && mapObj.Level - Level < 3;

        public bool IsRed(YiObj mapObj) => mapObj.Level - Level >= -4 && mapObj.Level - Level < 0;

        public bool IsBlack(YiObj mapObj) => mapObj.Level - Level < -4;

        public virtual void GetHealed(Player player, int value)
        {
            if (!Alive)
                return;
            CurrentHp += value;
            if (CurrentHp > MaximumHp)
                CurrentHp = MaximumHp;
        }

        public virtual void GetBuffed(Player player, int value)
        {
            if (!Alive)
                return;
            EntityLogic.Recalculate(player);
        }

        public unsafe void Jump(Vector2 waypoint)
        {
            var buffer = MsgAction.Jump(this, waypoint.X, waypoint.Y);

            MsgAction packet;
            fixed (byte* p = buffer)
                packet = *(MsgAction*)p;

            var x = (ushort)packet.Param;
            var y = (ushort)(packet.Param >> 16);

            ScreenSystem.Send(this, packet, false, true, packet.Size);

            Direction = Position.GetDirection(Location.X, Location.Y, waypoint.X, waypoint.Y);
            Location.X = waypoint.X;
            Location.Y = waypoint.Y;

            //if (TeamSystem.Teams.ContainsKey(UniqueId))
            //    TeamSystem.Teams[UniqueId].UpdateLeaderPosition(this as Player);

            ScreenSystem.Update(this);
        }

        public bool CanUseScroll() => !GameWorld.Maps.TryGetValue(MapId, out var map) || map.Flags.HasFlag(MapFlags.DisableScrolls);
    }
}
