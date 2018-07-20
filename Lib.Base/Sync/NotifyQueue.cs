using System;
using System.Timers;

namespace Lib.Base
{
    public class NotifyQueue<T>
    {
        public event EventHandler<EventArgsT<T>> Dequeue;

        private readonly SyncQueue<T> _q = new SyncQueue<T>();

        public void Enqueue(T item)
        {
            _q.Enqueue(item);
        }

        private void OnDequeue(EventArgsT<T> e)
        {
            var handler = Dequeue;
            if (handler != null) handler(this, e);
        }
    }
}