using Yi.Entities;
using Yi.Scheduler;

namespace Yi.AI
{
    public class BasicBrain : Brain
    {
        public override int ThinkInterval { get; set; } = 750;
        public BasicBrain(Monster owner):base(owner)
        {
            Owner = owner;
            ThinkJob = new Job(ThinkInterval, Think);
        }

        public override void Think()
        {
            if (!Owner.Alive || !Active)
                return;

            YiScheduler.Instance.Do(ThinkInterval, ThinkJob);

            base.Think();
        }
    }
}
