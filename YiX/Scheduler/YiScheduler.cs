using System;
using System.Collections.Generic;
using System.Threading;
using YiX.Enums;

namespace YiX.Scheduler
{
    public class YiScheduler : IYiEngine
    {
        #region Properties

        private readonly int _checkQueueIntervalMs;
        private int _counter;
        private readonly int _largeDelayQueueCutoffMs;
        private readonly int _moderateDelayQueueCutoffMs;
        private readonly int _queueIdleSleepTimeMs;
        private readonly int _totalThreads;
        public long WorkItemCount;
        private WorkItemQueueHandler[] _workItemQueueHandlers;
        internal ThreadSafeQueue<IExecutableWorkItem>[] AllWorkItemQueues;

        #endregion

        private static YiScheduler _instance;
        public static YiScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new YiScheduler(Environment.ProcessorCount*2);
                    _instance.StartSchedulingEngine();
                }
                return _instance;
            }
        }
        public DateTime CurrentDateTime { get; set; }
        public bool WantExit { get; set; }
        public ISupportsCount[] WorkItemQueueCounts => AllWorkItemQueues.ToInterfaceArray<ISupportsCount, ThreadSafeQueue<IExecutableWorkItem>>();
        public long WorkItemsExecuted { get; set; }

        public YiScheduler(int workerThreadCount)
        {
            _totalThreads = workerThreadCount;
            _moderateDelayQueueCutoffMs = 500;
            _largeDelayQueueCutoffMs = 3000;
            _checkQueueIntervalMs = 8;
            _queueIdleSleepTimeMs = 4;
            Initialize();
        }

        public void Do(SchedulerPriority priority, Action task)
        {
            switch (priority)
            {
                case SchedulerPriority.High:
                    Do(TimeSpan.MinValue, task);
                    break;
                case SchedulerPriority.Medium:
                    Do(TimeSpan.FromMilliseconds(100), task);
                    break;
                case SchedulerPriority.MediumLow:
                    Do(TimeSpan.FromMilliseconds(500), task);
                    break;
                case SchedulerPriority.Low:
                    Do(TimeSpan.FromMilliseconds(900), task);
                    break;
            }
        }

        public void Do(TimeSpan delay, Action action)
        {
            var job = new Job((int)delay.TotalMilliseconds, action);
            var queueNum = FindAppropriateQueue(job);
            AllWorkItemQueues[queueNum].Enqueue(job);
            Interlocked.Increment(ref WorkItemCount);
        }

        public Job DoReturn(TimeSpan delay, Action action)
        {
            var job = new Job((int)delay.TotalMilliseconds, action);
            var queueNum = FindAppropriateQueue(job);
            AllWorkItemQueues[queueNum].Enqueue(job);
            Interlocked.Increment(ref WorkItemCount);
            return job;
        }

        public void Do(Job action)
        {
            var queueNum = FindAppropriateQueue(action);
            AllWorkItemQueues[queueNum].Enqueue(action);
            Interlocked.Increment(ref WorkItemCount);
        }

        public void Do(int delay, Job action)
        {
            action.ExecutionTime = DateTime.UtcNow.AddMilliseconds(delay);
            var queueNum = FindAppropriateQueue(action);
            AllWorkItemQueues[queueNum].Enqueue(action);
            Interlocked.Increment(ref WorkItemCount);
        }

        public void StartSchedulingEngine()
        {
            for (var n = 0; n < AllWorkItemQueues.Length; n++)
            {
                AllWorkItemQueues[n] = new ThreadSafeQueue<IExecutableWorkItem>();
                _workItemQueueHandlers[n] = new WorkItemQueueHandler(this, n);
                _workItemQueueHandlers[n].StartEventHandler();
            }
        }

        internal void HandleWorkItemQueue(int n)
        {
            var lastUpdateQ = DateTime.UtcNow;

            while (!WantExit)
            {
                try
                {
                    var currentTime = CurrentDateTime = DateTime.UtcNow;

                    // If this is a immediate worker thread (work items scheduled to be executed soon)
                    if (n < _totalThreads - 2)
                    {
                        if (lastUpdateQ.AddMilliseconds(_checkQueueIntervalMs) <= currentTime)
                        {
                            lastUpdateQ = currentTime;
                            var executedThisPass = ExecuteEventsInQueue(n);

                            // If there was nothing to execute, sleep
                            if (executedThisPass == 0)
                                Thread.Sleep(_checkQueueIntervalMs);
                        }
                        else
                            Thread.Sleep(_queueIdleSleepTimeMs);
                    }
                    else if (n == _totalThreads - 2)
                    {
                        // This holds the "medium-term" work items - scheduled to be executed somewhat soon, but not immediately
                        if (lastUpdateQ.AddMilliseconds(_moderateDelayQueueCutoffMs - 50) <= currentTime)
                        {
                            lastUpdateQ = currentTime;
                            var updated = UpdateWorkItemSchedule(n);

                            if (updated == 0)
                                Thread.Sleep(100);
                        }
                        else
                            Thread.Sleep(50);
                    }
                    else if (n == _totalThreads - 1)
                    {
                        // This holds the "long-term" work items - scheduled to be executed later
                        if (lastUpdateQ.AddMilliseconds(_largeDelayQueueCutoffMs - 100) <= currentTime)
                        {
                            lastUpdateQ = currentTime;
                            var updated = UpdateWorkItemSchedule(n);

                            if (updated == 0)
                                Thread.Sleep(200);
                        }
                        else
                            Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine("--- YiScheduler.HandleWorkItemQueue ---");
                    Output.WriteLine(ex);
                }
            }
        }

        private int ExecuteEventsInQueue(int queueNumber)
        {
            var currentTime = CurrentDateTime;
            var reschedule = new List<IExecutableWorkItem>();
            var itemsToExecute = new List<IExecutableWorkItem>(16);
            var executedThisPass = 0;

            while (AllWorkItemQueues[queueNumber].DequeueMultiple(itemsToExecute, 10) > 0)
            {
                for (var n = 0; n < itemsToExecute.Count && !WantExit; n++)
                {
                    var item = itemsToExecute[n];

                    if (item.ExecutionTime > currentTime)
                        reschedule.Add(item);
                    else
                    {
                        executedThisPass++;
                        if (!item.Cancelled)
                            item.Execute();
                    }
                }

                itemsToExecute.Clear();
            }

            AllWorkItemQueues[queueNumber].EnqueueMultiple(reschedule);

            var execThisPass = Convert.ToInt64(executedThisPass);
            Interlocked.Add(ref WorkItemCount, -execThisPass);

            //WorkItemCount = 0;
            //foreach (var queue in AllWorkItemQueues)
            //{
            //    WorkItemCount += queue.Count;
            //}

            return executedThisPass;
        }

        private int FindAppropriateQueue(IExecutableWorkItem workItemToExecute)
        {
            var ts = workItemToExecute.ExecutionTime.Subtract(CurrentDateTime);
            if (ts.TotalMilliseconds > _largeDelayQueueCutoffMs)
                return _totalThreads - 1;
            if (ts.TotalMilliseconds > _moderateDelayQueueCutoffMs)
                return _totalThreads - 2;
            return Interlocked.Increment(ref _counter) % (_totalThreads - 2);
        }

        private void Initialize()
        {
            AllWorkItemQueues = new ThreadSafeQueue<IExecutableWorkItem>[_totalThreads];
            _workItemQueueHandlers = new WorkItemQueueHandler[_totalThreads];
            CurrentDateTime = DateTime.UtcNow;
        }

        private int UpdateWorkItemSchedule(int queueNumber)
        {
            var workItemsMoved = 0;
            var itemsBackIntoOriginalQueue = new List<IExecutableWorkItem>();
            var workItems = new List<IExecutableWorkItem>(16);

            while (AllWorkItemQueues[queueNumber].DequeueMultiple(workItems, 10) > 0 && !WantExit)
            {
                foreach (var item in workItems)
                {
                    // Determine the appropriate work item queue for this item, based on its scheduled execution time
                    var appropriateQueue = FindAppropriateQueue(item);

                    // Check if this item needs to be moved into a different queue
                    if (queueNumber != appropriateQueue)
                    {
                        AllWorkItemQueues[appropriateQueue].Enqueue(item);
                        workItemsMoved++;
                    }
                    else
                    {
                        // We will need to put the item back into the original queue
                        itemsBackIntoOriginalQueue.Add(item);
                    }
                }

                workItems.Clear();
            }

            // Return the work items that did not need to be moved to the slow-track queue
            AllWorkItemQueues[queueNumber].EnqueueMultiple(itemsBackIntoOriginalQueue);

            return workItemsMoved;
        }
    }

    public interface ISupportsCount
    {
    }
}