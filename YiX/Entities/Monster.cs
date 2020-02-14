using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using YiX.AI;
using YiX.Database;
using YiX.Database.Squiggly;
using YiX.Enums;
using YiX.Helpers;
using YiX.Network.Packets.Conquer;
using YiX.Scheduler;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Entities
{
    [Serializable]
    public class Monster : YiObj
    {
        [JsonIgnore]
        public Brain Brain;
        public int Id { get; set; }
        public short SpawnId { get; set; }
        public Vector2 SpawnPoint { get; set; }
        public DateTime LastAttack { get; set; }
        public DateTime LastMove { get; set; }
        public DateTime SpawnTime { get; set; }
        public Drops Drops { get; set; }
        public long AIType { get; set; }
        public int CQAction { get; set; }
        public long AttackUser { get; set; }
        public long RunSpeed { get; set; }
        public long WalkSpeed { get; set; }
        public long EscapeLife { get; set; }
        public long ViewRange { get; set; }
        public long Dodge { get; set; }

        public DateTime NoTargetSince;
        [JsonIgnore]
        public readonly Job DespawnJob;
        [JsonIgnore]
        public Job RespawnJob;
        [JsonIgnore]
        public readonly Queue<Vector2> Waypoints = new Queue<Vector2>();

        public Monster()
        {
            Brain = new BasicBrain(this);
            DespawnJob = new Job(3000, () => ScreenSystem.Send(this, MsgAction.Create(this, 0, MsgActionType.EntityRemove)));
        }

        public override void KilledFrom(YiObj attacker)
        {
            base.KilledFrom(attacker);
            AddStatusEffect(StatusEffect.Die | StatusEffect.Frozen | StatusEffect.Fade);
            YiScheduler.Instance.Do(3000, DespawnJob);
            RespawnJob = new Job(Collections.Spawns[SpawnId].RespawnDelay * 1000 * 30, Respawn);
            YiScheduler.Instance.Do(Collections.Spawns[SpawnId].RespawnDelay * 1000 * 15, RespawnJob);
            GameWorld.Maps[MapId].RemoveMob(this);
            MobDropSystem.Drop(attacker, this);
            ConquerActionProcessor.ExecuteAction(this, attacker);
        }

        public override void Respawn()
        {
            FindRespawnLocation();
            SpawnTime = DateTime.UtcNow;
            GameWorld.Maps[MapId].AddMob(this);
            StatusEffects = 0;
            CurrentHp = MaximumHp;
            ScreenSystem.Update(this);
            ScreenSystem.SendSpawn(this);
            YiScheduler.Instance.Do(Brain.ThinkInterval, Brain.ThinkJob);
        }

        private void FindRespawnLocation()
        {
            var spawn = Collections.Spawns[SpawnId];

            var spawnAttempts = 0;
            while (true)
            {
                if (spawnAttempts > 1000)
                    break;

                Location.X = (ushort)SafeRandom.Next(spawn.Xstart - 10, spawn.Xstart + spawn.Xend + 10);
                Location.Y = (ushort)SafeRandom.Next(spawn.Ystart - 10, spawn.Ystart + spawn.Yend + 10);

                spawnAttempts++;
                if (GameWorld.Maps[MapId].MobValid(Location.X, Location.Y))
                {
                    break;
                }
            }
            SpawnPoint = Location;
        }

        public override string ToString() => Name;
    }
}