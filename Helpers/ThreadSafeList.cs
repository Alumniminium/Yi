using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YiX.Helpers
{
    public sealed class ThreadSafeList<T> : IList<T>
    {
        private readonly List<T> _items = new List<T>();

        public ThreadSafeList(IEnumerable<T> items = null)
        {
            AddRange(items);
        }

        public long LongCount
        {
            get
            {
                lock (_items)
                    return _items.LongCount();
            }
        }

        public int Count
        {
            get
            {
                lock (_items)
                    return _items.Count;
            }
        }

        public bool IsReadOnly => false;

        public T this[uint index]
        {
            get
            {
                lock (_items)
                    return _items[(int)index];
            }

            set
            {
                lock (_items)
                    _items[(int)index] = value;
            }
        }
        public T this[int index]
        {
            get
            {
                lock (_items)
                    return _items[index];
            }

            set
            {
                lock (_items)
                    _items[index] = value;
            }
        }

        public void Add(T item)
        {
            lock (_items)
                if (!_items.Contains(item))
                    _items.Add(item);
        }

        public void Clear()
        {
            lock (_items)
                _items.Clear();
        }

        public bool Contains(T item)
        {
            lock (_items)
                return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_items)
                _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() => Clone().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item)
        {
            lock (_items)
                return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (_items)
                _items.Insert(index, item);
        }

        public bool Remove(T item)
        {
            lock (_items)
                return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            lock (_items)
                _items.RemoveAt(index);
        }

        public void AddRange(IEnumerable<T> collection, bool asParallel = true)
        {
            if (null == collection)
                return;
            lock (_items)
            {
                _items.AddRange(asParallel ? collection.AsParallel() : collection);
            }
        }

        public List<T> Clone(bool asParallel = true)
        {
            lock (_items)
            {
                return asParallel ? new List<T>(_items.AsParallel()) : new List<T>(_items);
            }
        }
    }
}