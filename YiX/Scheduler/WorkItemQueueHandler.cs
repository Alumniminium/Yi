using System.Threading;

namespace YiX.Scheduler
{
    internal class WorkItemQueueHandler
    {
        #region Properties

        private readonly YiScheduler _yiScheduler;
        internal readonly int HandlesQueueNumber;
        internal ThreadStart ThreadStart;
        internal Thread WorkerThread;

        #endregion

        public WorkItemQueueHandler(YiScheduler engine, int handlesQueueNumber)
        {
            _yiScheduler = engine;
            HandlesQueueNumber = handlesQueueNumber;
        }

        public void HandleAndExecuteWorkItems() => _yiScheduler.HandleWorkItemQueue(HandlesQueueNumber);

        public void StartEventHandler()
        {
            ThreadStart = HandleAndExecuteWorkItems;
            WorkerThread = ThreadStart.CreateAndRunThread();
        }
    }
}