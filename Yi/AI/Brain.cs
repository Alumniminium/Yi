using System;
using System.Linq;
using Newtonsoft.Json;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.SelfContainedSystems;
using Yi.Structures;
using Yi.World;

namespace Yi.AI
{
    public class Brain
    {
        [JsonIgnore]
        public Job ThinkJob;
        [JsonIgnore]
        private int _playersInSight;

        [JsonIgnore]
        protected virtual bool Active
        {
            get
            {
                if (!ScreenSystem.GetEntities(Owner).OfType<Player>().Any())
                    PlayersInSight = 0;
                return PlayersInSight > 0;
            }
        }

        [JsonIgnore]
        public int PlayersInSight
        {
            get => _playersInSight;
            set
            {
                if (value == 1)
                {
                    YiScheduler.Instance.Do(ThinkJob);
                }
                _playersInSight = value;
            }
        }

        public virtual int ThinkInterval { get; set; } = 500;

        protected Monster Owner;

        protected Brain(Monster owner)
        {
            Owner = owner;
            ThinkJob = new Job(350, Think);
        }

        public virtual void Think()
        {
            if (!FoundTarget())
                return;

            if (NeedsToMove())
            {
                if (CanMove())
                    Move();
            }
            else
            {
                if (AttackReady())
                    Attack();
            }
        }

        protected virtual bool NeedsToMove() => !Position.InAttackRange(Owner, Owner.CurrentTarget);

        protected virtual bool AttackReady() => DateTime.UtcNow > Owner.LastAttack.AddSeconds(2) && DateTime.UtcNow > Owner.LastMove.AddMilliseconds(700) && Owner.SpawnTime.AddSeconds(2) < DateTime.UtcNow;


        public virtual void Attack()
        {
            int damage;

            if (Owner.AttackRange > 2)
            {
                Owner.CurrentSkill = new Skill(1000, 0, 0);//Thunder
                damage = AttackCalcs.MagicDmg(Owner, Owner.CurrentTarget);

                ScreenSystem.Send(Owner, MsgInteract.Create(Owner, Owner.CurrentTarget, MsgInteractType.Magic, damage));
                ScreenSystem.Send(Owner, MsgMagicEffect.Create(Owner, Owner.CurrentTarget, damage));
            }
            else
            {
                damage = AttackCalcs.GetDamage(Owner, Owner.CurrentTarget, MsgInteractType.Physical);
                ScreenSystem.Send(Owner, MsgInteract.Create(Owner, Owner.CurrentTarget, MsgInteractType.Physical, damage));
            }
            Owner.LastAttack = DateTime.UtcNow;
            Owner.CurrentTarget.GetHit(Owner, damage);
        }

        protected virtual bool FoundTarget()
        {
            foreach (var potentialTarget in ScreenSystem.GetEntities(Owner).OfType<Player>())
            {
                if (Owner.Name.Contains("Guard"))
                {
                    if (!potentialTarget.HasFlag(StatusEffect.Flashing) && !potentialTarget.HasFlag(StatusEffect.BlackName))
                        continue;
                }

                var distanceToPotentialTarget = Position.GetDistance(Owner, potentialTarget);

                if (distanceToPotentialTarget > 19)
                    continue;

                if (potentialTarget.HasFlag(StatusEffect.SpawnProtection) || potentialTarget.HasFlag(StatusEffect.Flying) || potentialTarget.HasFlag(StatusEffect.Invisibility) || potentialTarget.HasFlag(StatusEffect.Die))
                    continue;

                Owner.CurrentTarget = potentialTarget;
                return true;
            }
            return false;
        }

        protected virtual void Move()
        {
            if (Owner.Waypoints.Count <= 0 || Position.GetDistance(Owner.Location, Owner.CurrentTarget.Location) <= 1)
                return;

            var waypoint = Owner.Waypoints.Dequeue();
            var direction = (Direction)Position.GetDirectionCo(Owner.Location.X, Owner.Location.Y, waypoint.X, waypoint.Y);
            var walkPacket = MsgWalk.Create(Owner.UniqueId, direction, true);


            GameWorld.Maps[Owner.MapId].MoveMob(Owner.Location, waypoint);
            Owner.Location = waypoint;
            Owner.Direction = direction;
            ScreenSystem.Send(Owner, walkPacket);
            ScreenSystem.Update(Owner);
            Owner.LastMove = DateTime.UtcNow;
        }

        public virtual bool CanMove() => CanMove(Owner.CurrentTarget.Location) && DateTime.UtcNow >= Owner.LastMove.AddSeconds(1) && Owner.SpawnTime.AddSeconds(2) < DateTime.UtcNow;

        public virtual bool CanMove(Vector2 targetLocation)
        {
            Owner.Waypoints.Clear();
            var path = GameWorld.Maps[Owner.MapId].Path(Owner.Location, targetLocation);

            if (path == null)
                return false;

            foreach (var pos in path.Where(pos => pos != Owner.CurrentTarget.Location))
                Owner.Waypoints.Enqueue(pos);

            return true;
        }
    }
}