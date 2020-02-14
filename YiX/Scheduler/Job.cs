using System;

namespace YiX.Scheduler
{
    [Serializable]
    public class Job : IExecutableWorkItem
    {
        #region Properties

        public readonly Action Action;

        #endregion

        public bool Cancelled { get; set; }
        public DateTime ExecutionTime { get; set; }

        public Job(int ms, Action action)
        {
            Cancelled = false;
            Action = action;
            ExecutionTime = DateTime.UtcNow.AddMilliseconds(ms);
        }

        public Job(DateTime startTime, Action action)
        {
            Cancelled = false;
            Action = action;
            ExecutionTime = startTime;
        }

        public Job(TimeSpan startTime, Action action)
        {
            Action = action;
            Cancelled = false;
            ExecutionTime = DateTime.UtcNow.Add(startTime);
        }

        public void Execute() => Action?.Invoke();
    }
}