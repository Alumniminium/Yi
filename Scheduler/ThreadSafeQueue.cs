using System.Collections.Generic;
using System.ComponentModel;

namespace YiX.Scheduler
{
    [Browsable(true), TypeConverter(typeof(ExpandableObjectConverter))]
    internal sealed class ThreadSafeQueue<TListType> :Lockable, ISupportsCount, IMultipleItems<TListType>, ISupportsEnqueueDequeue<TListType> where TListType : class
    {
        #region Properties

        public readonly Queue<TListType> Queue = new Queue<TListType>();

        #endregion

        public HashSet<TListType> AllItems => new HashSet<TListType>(ArrayOfItems);
        public IEnumerable<TListType> ArrayOfItems
        {
            get
            {
                TListType[] list;

                AquireLock();
                {
                    list = Queue.ToArray();
                }
                ReleaseLock();

                return list;
            }
        }
        public int Count { get; private set; }

        public void Clear()
        {
            AquireLock();
            {
                Queue.Clear();
                Count = 0;
            }
            ReleaseLock();
        }

        public bool Dequeue(out TListType item)
        {
            item = null;

            // We need to be sure that no other threads simultaneously modify the shared _queue
            // object during our dequeue operation
            AquireLock();
            {
                if (Count > 0)
                {
                    item = Queue.Dequeue();
                    Count--;
                }
            }
            ReleaseLock();

            return item != null;
        }

        public int DequeueMultiple(List<TListType> items, int maxItems)
        {
            var dequeued = 0;

            AquireLock();
            {
                while (Count > 0 && dequeued < maxItems)
                {
                    var item = Queue.Dequeue();
                    items.Add(item);
                    Count--;
                    dequeued++;
                }
            }
            ReleaseLock();

            return dequeued;
        }

        public void Enqueue(TListType item)
        {
            // We need to be sure that no other threads simultaneously modify the shared _queue
            // object during our enqueue operation
            AquireLock();
            {
                Queue.Enqueue(item);
                Count++;
            }
            ReleaseLock();
        }

        public void EnqueueMultiple(List<TListType> items)
        {
            AquireLock();
            {
                foreach (var item in items)
                    Queue.Enqueue(item);

                Count += items.Count;
            }
            ReleaseLock();
        }

        public bool IsInList(TListType item)
        {
            bool found;

            AquireLock();
            {
                found = Queue.Contains(item);
            }
            ReleaseLock();

            return found;
        }
    }
}