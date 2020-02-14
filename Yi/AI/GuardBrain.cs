using System;
using System.Linq;
using Yi.Calculations;
using Yi.Entities;
using Yi.Enums;
using Yi.Network.Packets.Conquer;
using Yi.Scheduler;
using Yi.SelfContainedSystems;
using Yi.Structures;

namespace Yi.AI
{
    public class GuardBrain : Brain
    {
        public GuardBrain(Monster monster) : base(monster)
        {
            ThinkJob = new Job(350, Think);
        }

        public override void Think()
        {
            if (!Owner.Alive || !Active)
                return;

            YiScheduler.Instance.Do(350, ThinkJob);

            base.Think();
        }

        protected override bool FoundTarget()
        {
            foreach (var potentialTarget in ScreenSystem.GetEntities(Owner).OfType<Player>())
            {
                if (!potentialTarget.HasFlag(StatusEffect.Flashing) && !potentialTarget.HasFlag(StatusEffect.BlackName))
                    continue;

                var distanceToPotentialTarget = Calculations.Position.GetDistance(Owner, potentialTarget);

                if (distanceToPotentialTarget > 12)
                    continue;

                if (potentialTarget.HasFlag(StatusEffect.Flying) || potentialTarget.HasFlag(StatusEffect.Invisibility) || potentialTarget.HasFlag(StatusEffect.Die))
                    continue;

                Owner.CurrentTarget = potentialTarget;
                return true;
            }
            foreach (var potentialTarget in ScreenSystem.GetEntities(Owner).OfType<Monster>())
            {
                var distanceToPotentialTarget = Calculations.Position.GetDistance(Owner, potentialTarget);

                if (distanceToPotentialTarget > 11)
                    continue;

                if (potentialTarget.HasFlag(StatusEffect.Flying) || potentialTarget.HasFlag(StatusEffect.Invisibility) || potentialTarget.HasFlag(StatusEffect.Die))
                    continue;

                Owner.CurrentTarget = potentialTarget;
                return true;
            }
            return false;
        }

        public override bool CanMove() => false;

        public override void Attack()
        {
            Owner.CurrentSkill = new Skill((ushort) Owner.MagicType, 0, 0); //Thunder

            ScreenSystem.Send(Owner, MsgInteract.Create(Owner, Owner.CurrentTarget, MsgInteractType.Magic, 133337));
            ScreenSystem.Send(Owner, MsgMagicEffect.Create(Owner, Owner.CurrentTarget, 133337));

            Owner.LastAttack = DateTime.UtcNow;
            Owner.CurrentTarget.GetHit(Owner, 133337);
        }
    }
}