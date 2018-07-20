using System.Collections.Generic;

namespace Lib.Base
{
    public class SyncDistinctQueue<T> : SmartSyncQueue<T>
    {
        public new T Dequeue()
        {
            lock (SyncRoot)
            {
                while (true)
                {
                    var dequeued = base.Dequeue();
                    if (Count == 0)
                        return dequeued;

                    var peek = Peek();
                    if (dequeued.Equals(peek))
                        continue;
                    return dequeued;
                }
            }
        }

        public List<T> DequeueAll()
        {
            List<T> ret = new List<T>();

            T result;
            while (TryDequeue(out result))
                ret.Add(result);

            return ret;
        }

        public new bool TryDequeue(out T result)
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
    }
}