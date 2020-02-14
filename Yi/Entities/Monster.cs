using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Yi.AI;
using Yi.Database;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.Entities
{
    [Serializable]
    public class Monster : YiObj
    {
        [JsonIgnore]
        public override string Name => Collections.MonsterNames[Id];
        [JsonIgnore]
        public Brain Brain;
        public uint Id { get; set; }
        public short SpawnId { get; set; }
        public Vector2 SpawnPoint { get; set; }
        public DateTime LastAttack { get; set; }
        public DateTime LastMove { get; set; }
        public DateTime SpawnTime { get; set; }
        public Drops Drops { get; set; }
        public DateTime NoTargetSince;
        [JsonIgnore]
        public readonly Job DespawnJob;
        [JsonIgnore]
        public readonly Job RespawnJob;
        [JsonIgnore]
        public readonly Queue<Vector2> Waypoints = new Queue<Vector2>();

        public Monster()
        {
            Brain = new BasicBrain(this);
            DespawnJob = new Job(3000, () => ScreenSystem.Send(this, MsgAction.Create(this, 0, MsgActionType.EntityRemove)));
            RespawnJob = new Job(Collections.Spawns[SpawnId].RespawnDelay * 1000 * 30, Respawn);
        }

        public override void KilledFrom(YiObj attacker)
        {
            base.KilledFrom(attacker);
            AddStatusEffect(StatusEffect.Die | StatusEffect.Frozen | StatusEffect.Fade);
            YiScheduler.Instance.Do(3000, DespawnJob);
            YiScheduler.Instance.Do(Collections.Spawns[SpawnId].RespawnDelay * 1000 * 15, RespawnJob);
            GameWorld.Maps[MapId].RemoveMob(this);
            MobDropSystem.Drop(attacker, this);
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

                Location.X = (ushort) YiCore.Random.Next(spawn.Xstart - 10, spawn.Xstart + spawn.Xend + 10);
                Location.Y = (ushort) YiCore.Random.Next(spawn.Ystart - 10, spawn.Ystart + spawn.Yend + 10);

                spawnAttempts++;
                if (GameWorld.Maps[MapId].MobValid(Location.X, Location.Y))
                {
                    break;
                }
            }
            SpawnPoint = Location;
        }

        public override string ToString() => Collections.BaseMonsters[Id].Name;
    }
}