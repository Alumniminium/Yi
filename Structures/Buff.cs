using System;
using YiX.Entities;
using YiX.Enums;
using YiX.Scheduler;
using YiX.SelfContainedSystems;

namespace YiX.Structures
{
    public class Buff
    {
        public SkillId SkillId;
        public string Description = "TestBuff";
        public TimeSpan Duration;
        public DateTime StartTime,EndTime;
        public StatusEffect Effect = StatusEffect.None;
        public float PhysAtkMod = 1.0f;
        public float MagicAtkMod = 1.0f;
        public float PhysDefMod = 1.0f;
        public float MagicDefMod = 1.0f;
        public float AccuracyMod = 1.0f;
        public float SpellExpMod = 1.0f;
        public float LevelExpMod = 1.0f;
        public float LuckMod = 1.0f;
        public float SpeedMod = 1.0f;
        public Job RemoveJob;

        public Buff(YiObj owner,SkillId id, TimeSpan duration)
        {
            SkillId = id;
            Duration = duration;
            StartTime = DateTime.UtcNow;
            EndTime = DateTime.UtcNow.Add(Duration);
            RemoveJob = YiScheduler.Instance.DoReturn(Duration, () => BuffSystem.RemoveBuff(owner, this));
        }

        public void AddTime(int ms)
        {
            if(SkillId== SkillId.Superman|| SkillId== SkillId.Cyclone)
                if((EndTime - DateTime.UtcNow).Seconds > 25)
                    return;

            RemoveJob.ExecutionTime = EndTime = EndTime.AddMilliseconds(ms);
        }

        public string GetTimeLeft() => (EndTime- DateTime.UtcNow).ToString(@"hh\:mm\:ss");
        public override string ToString() => $"[{GetTimeLeft()}] {Description}";
    }
}