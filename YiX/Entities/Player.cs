using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using YiX.Calculations;
using YiX.Database;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Packets.Conquer;
using YiX.Network.Sockets;
using YiX.Scheduler;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Entities
{
    [Serializable]
    public class Player : YiObj
    {
        [JsonIgnore]
        public bool Online;
        [JsonIgnore]
        public ClientSocket GameSocket;
        [JsonIgnore]
        private int _lastTick;
        [JsonIgnore]
        public int CurrentNpcId { get; set; }
        [JsonIgnore]
        public Job SpawnProtectionTimer { get; set; }
        [JsonIgnore]
        public Job XpIncrementJob { get; set; }
        [JsonIgnore]
        public Job FlashingJob { get; set; }
        [JsonIgnore]
        public Job PkPJob { get; set; }
        [JsonIgnore]
        public int FriendRequestTarget { get; set; }
        [JsonIgnore]
        public Job AttackJob { get; set; }
        [JsonIgnore]
        public int LastTick
        {
            get => _lastTick;
            set
            {
                if (value - _lastTick - 100000 > 1500)
                    Send(MsgText.Create("Server", Name, "High Ping!", MsgTextType.Top));
                _lastTick = value;
            }
        }

        public string IP { get; set; }
        public string AccountId { get; set; }
        public string Password { get; set; }
        public LoginType LoginType { get; set; }

        public Monster Pet { get; set; }

        public Dictionary<int, int> NpcTasks = new Dictionary<int, int>();

        public uint CurrentKO { get; set; }
        public int JoinGuildRequest { get; set; }

        public override byte Level
        {
            get => base.Level;
            set
            {
                base.Level = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Level));
                ScreenSystem.Send(this, MsgAction.LevelUp(this), true);

                CurrentHp = MaximumHp;
                CurrentMp = MaximumMp;
            }
        }

        public override uint Look
        {
            get => base.Look;
            set
            {
                base.Look = value;
                ScreenSystem.Send(this,MsgUpdate.Create(this, value, MsgUpdateType.Model),true);
            }
        }
        public override uint Experience
        {
            get => base.Experience;
            set
            {
                var expAdd = value - base.Experience;
                expAdd = (uint)(expAdd* YiCore.ExpMulti);
                base.Experience = base.Experience + expAdd;
                if (Level >= 130)
                {
                    base.Experience = (uint)Collections.LevelExps[130].ExpReq;
                    return;
                }
                if (Level < 1)
                    Level = 1;
                if (Collections.LevelExps[Level].ExpReq <= base.Experience)
                {
                    base.Experience = 0;
                    Level++;
                    EntityLogic.Recalculate(this);
                }
                Send(MsgUpdate.Create(this, base.Experience, MsgUpdateType.Exp));
            }
        }
        public override byte Class
        {
            get => base.Class;
            set
            {
                base.Class = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Class));
            }
        }
        public override int CurrentHp
        {
            get => base.CurrentHp;
            set
            {
                base.CurrentHp = Math.Max(0, value);
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Hp));
            }
        }
        public override ushort MaximumHp
        {
            get => base.MaximumHp;
            set
            {
                base.MaximumHp = Math.Max((ushort)1, value);
                Send(MsgUpdate.Create(this, value, MsgUpdateType.MaxHp));
            }
        }
        public override ushort CurrentMp
        {
            get => base.CurrentMp;
            set
            {
                base.CurrentMp = Math.Max((ushort)0, value);
                if (Class >= 100)
                    Send(MsgUpdate.Create(this, value, MsgUpdateType.CurrentMana));
            }
        }
        public override ushort MaximumMp
        {
            get => base.MaximumMp;
            set
            {
                base.MaximumMp = value;
                if (Class >= 100)
                    Send(MsgUpdate.Create(this, value, MsgUpdateType.MaxMana));
            }
        }
        public override int Money
        {
            get => base.Money;
            set
            {
                base.Money = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.InvMoney));
            }
        }
        public override byte Reborn
        {
            get => base.Reborn;
            set
            {
                base.Reborn = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Metempsychosis));
            }
        }
        public override ushort Agility
        {
            get => base.Agility;
            set
            {
                base.Agility = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Agility));
            }
        }
        public override ushort Spirit
        {
            get => base.Spirit;
            set
            {
                base.Spirit = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Spirit));
            }
        }
        public override ushort Strength
        {
            get => base.Strength;
            set
            {
                base.Strength = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Strength));
            }
        }
        public override ushort Vitality
        {
            get => base.Vitality;
            set
            {
                base.Vitality = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Vitality));
            }
        }
        public override ushort Statpoints
        {
            get => base.Statpoints;
            set
            {
                base.Statpoints = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.StatPoints));
            }
        }
        public override byte Stamina
        {
            get => base.Stamina;
            set
            {
                base.Stamina = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.Stamina));
            }
        }

        public override ushort PkPoints
        {
            get => base.PkPoints;
            set
            {
                base.PkPoints = value;
                Send(MsgUpdate.Create(this, value, MsgUpdateType.PkPoints));
            }
        }

        public Player(ClientSocket socket)
        {
            GameSocket = socket;
            NpcTasks = new Dictionary<int, int>();
            Location = new Vector2(0, 0);
            Profs = new Dictionary<int, Prof>();
            Skills = new Dictionary<SkillId, Skill>();
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
            Friends = new List<int>();
            Enemies = new List<int>();
        }

        public void IncrementXp()
        {
            if (!HasFlag(StatusEffect.Die))
            {
                Interlocked.Increment(ref Xp);
                if (Xp >= 100)
                {
                    AddStatusEffect(StatusEffect.XpList);
                    Interlocked.Exchange(ref Xp, 0);
                }
            }
            XpIncrementJob = YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(3), IncrementXp);
        }

        public Player(bool setUniqueId = false)
        {
            if (setUniqueId)
                UniqueId = UniqueIdGenerator.GetNext(EntityType.Player);
            Location = new Vector2(0, 0);
            Profs = new Dictionary<int, Prof>();
            Skills = new Dictionary<SkillId, Skill>();
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
        }

        protected Player()
        {

        }

        public override void GetMessage(string from, string to, string message, MsgTextType type) => Send(MsgText.Create(from, to, message, type));

        public void RemoveSkill(Skill s)
        {
            Skills.Remove((SkillId) s.Id);
            Send(MsgAction.Create(this, s.Id, MsgActionType.DropMagic));
        }

        public void AddSkill(Skill s)
        {
            if (Skills.ContainsKey((SkillId)s.Id))
                RemoveSkill(s);

            Skills.Add((SkillId)s.Id, s);
            Send(MsgSkill.Create(s));
        }

        public void UpdateSkill(Skill s)
        {
            RemoveSkill(s);
            AddSkill(s);
        }

        public void AddSpawnProtection()
        {
            RemoveStatusEffect(StatusEffect.XpList);
            SpawnProtectionTimer = YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(5), RemoveSpawnProtection);
            for (var i = 5; i > 0; i--)
            {
                var num = i;
                SpawnProtectionTimer = YiScheduler.Instance.DoReturn(TimeSpan.FromSeconds(5 - num), () => Send(LegacyPackets.Effect(this, "downnumber" + num)));
            }
            AddStatusEffect(StatusEffect.SpawnProtection);
        }

        public override void KilledFrom(YiObj attacker)
        {
            base.KilledFrom(attacker);
            Xp = 0;

            if (HasFlag(StatusEffect.BlackName))
            {
                if (attacker is Player || attacker.Name.Contains("Guard"))
                {
                    Teleport(29, 72, 6000);
                    Message.Broadcast("SERVER", $"{attacker.Name.TrimEnd('\0') } has sent {Name.TrimEnd('\0') } to Jail!", MsgTextType.Center);
                }
            }

            if (attacker is Player playerAttacker)
            {
                if (GameWorld.Maps.TryGetValue(MapId, out var map))
                {
                    if (!map.Flags.HasFlag(MapFlags.NoPkpNoFlash))
                    {
                        if (!HasFlag(StatusEffect.BlackName) && !HasFlag(StatusEffect.Flashing))
                        {
                            if (attacker.Enemies.Contains(UniqueId))
                                playerAttacker.PkPoints += 3;
                            else
                                playerAttacker.PkPoints += 10;

                            if (playerAttacker.PkPoints > 29)
                                playerAttacker.AddStatusEffect(StatusEffect.RedName);
                            if (playerAttacker.PkPoints > 99)
                                playerAttacker.AddStatusEffect(StatusEffect.BlackName);
                        }
                    }
                }
            }

            RemoveStatusEffect(StatusEffect.Cyclone);
            RemoveStatusEffect(StatusEffect.Flying);
            RemoveStatusEffect(StatusEffect.CastingPray);
            AddStatusEffect(StatusEffect.Die);

            if (Look % 10000 == 2001 || Look % 10000 == 2002)
                AddTransform(99);
            else
                AddTransform(98);
        }

        public void RemoveSpawnProtection()
        {
            SpawnProtectionTimer.Cancelled = true;
            if (!HasFlag(StatusEffect.SpawnProtection))
                return;
            RemoveStatusEffect(StatusEffect.SpawnProtection);
        }
        public void ForceSend(byte[] packet, int size)
        {
            if (GameSocket?.Socket == null || !GameSocket.Socket.Connected || packet == null)
                return;
            GameSocket?.Send(packet, size);
        }

        public void Send(params byte[][] packets)
        {
            if (GameSocket?.Socket == null || !GameSocket.Socket.Connected)
                return;
            foreach (var packet in packets)
            {
                if (packet == null)
                    continue;
                OutgoingPacketQueue.Add(this, packet);
            }
        }

        public void Disconnect()
        {
            try
            {
                foreach (var friend in Friends)
                {
                    if (GameWorld.Find(friend, out Player found))
                        found.Send(MsgFriend.Create(this, MsgFriendActionType.FriendOffline, MsgFriendStatusType.Offline));
                }
                if (XpIncrementJob != null)
                    XpIncrementJob.Cancelled = true;
                Online = false;
                GameSocket?.Socket?.Dispose();
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        public override string ToString() => "[Player]" + UniqueId;

        public void SendRaw(byte[] packet)
        {

        }
    }
}