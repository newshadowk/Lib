using System.Collections.Generic;
using System.Threading;

namespace Lib.Base
{
    public class SyncQueue<T> : Queue<T>
    {
        private object _syncRoot;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }

        public bool TryDequeue(out T result)
        {
            lock (SyncRoot)
            {
                if (Count == 0)
                {
                    result = default(T);
                    return false;
                }

                result = Dequeue();
                return true;
            } 
        }

        public new void Enqueue(T item)
        {
            lock (SyncRoot)
                base.Enqueue(item);
        }

        public new T Dequeue()
        {
            lock (SyncRoot)
                return base.Dequeue();
        }

        public new T Peek()
        {
            lock (SyncRoot)
                return base.Peek();
        }

        public new void CopyTo(T[] array, int arrayIndex)
        {
            lock (SyncRoot)
                base.CopyTo(array, arrayIndex);
        }

        public new bool Contains(T item)
        {
            lock (SyncRoot)
                return base.Contains(item);
        }

        public new int Count
        {
            get
            {
                lock (SyncRoot)
                    return base.Count;
            }
        }

        public new T[] ToArray()
        {
            lock (SyncRoot)
                return base.ToArray();
        }

        public new void TrimExcess()
        {
            lock (SyncRoot)
                base.TrimExcess();
        }
    }
}