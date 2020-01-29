using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YiX.Helpers
{
    public class SafeList<T> : IList<T>
    {
        public readonly List<T> Items = new List<T>();

        public SafeList(IEnumerable<T> items = null)
        {
            AddRange(items);
        }

        public long LongCount
        {
            get
            {
                lock (Items)
                    return Items.LongCount();
            }
        }

        public int Count
        {
            get
            {
                lock (Items)
                    return Items.Count;
            }
        }

        public bool IsReadOnly => false;

        public T this[uint index]
        {
            get
            {
                lock (Items)
                    return Items[(int)index];
            }

            set
            {
                lock (Items)
                    Items[(int)index] = value;
            }
        }
        public T this[int index]
        {
            get
            {
                lock (Items)
                    return Items[index];
            }

            set
            {
                lock (Items)
                    Items[index] = value;
            }
        }

        public void Add(T item)
        {
            lock (Items)
                if (!Items.Contains(item))
                    Items.Add(item);
        }

        public void Clear()
        {
            lock (Items)
                Items.Clear();
        }

        public bool Contains(T item)
        {
            lock (Items)
                return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (Items)
                Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() => Clone().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item)
        {
            lock (Items)
                return Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (Items)
                Items.Insert(index, item);
        }

        public bool Remove(T item)
        {
            lock (Items)
                return Items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            lock (Items)
                Items.RemoveAt(index);
        }

        public void AddRange(IEnumerable<T> collection, bool asParallel = true)
        {
            if (null == collection)
                return;
            lock (Items)
            {
                Items.AddRange(asParallel ? collection.AsParallel() : collection);
            }
        }

        public List<T> Clone(bool asParallel = true)
        {
            lock (Items)
            {
                return asParallel ? new List<T>(Items.AsParallel()) : new List<T>(Items);
            }
        }
    }
}