using System;

namespace Lib.Base
{
    public class SyncProrityQueue<T>
    {
        private readonly SmartSyncQueue<T> _highQ = new SmartSyncQueue<T>();
        private readonly SmartSyncQueue<T> _normalQ = new SmartSyncQueue<T>();

        public void Enqueue(T item, QueuePrority prority)
        {
            var queue = GetQueue(prority);
            queue.Enqueue(item);
        }

        public bool TryDequeue(out T result, out QueuePrority prority)
        {
            prority = default(QueuePrority);

            if (_highQ.TryDequeue(out result))
            {
                prority = QueuePrority.High;
                return true;
            }

            if (_normalQ.TryDequeue(out result))
            {
                prority = QueuePrority.Normal;
                return true;
            }

            return false;
        }

        public void Clear(QueuePrority prority)
        {
            SmartSyncQueue<T> queue = GetQueue(prority);
            queue.Clear();
        }

        public int Count
        {
            get { return NormalCount + HighCount; }
        }

        public int HighCount
        {
            get { return _highQ.Count; }
        }

        public int NormalCount
        {
            get { return _normalQ.Count; }
        }

        private SmartSyncQueue<T> GetQueue(QueuePrority prority)
        {
            switch (prority)
            {
                case QueuePrority.High:
                    return _highQ;
                case QueuePrority.Normal:
                    return _normalQ;
                default:
                    throw new ArgumentOutOfRangeException("prority");
            }
        }
    }
}
