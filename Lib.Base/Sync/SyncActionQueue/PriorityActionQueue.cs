using System;
using System.Collections.Generic;
using System.Timers;

namespace Lib.Base
{
    public sealed class PriorityActionQueue
    {
        private readonly Queue<Action> _qHigh = new Queue<Action>();
        private readonly Queue<Action> _qLow = new Queue<Action>();
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BusyTimer _tLow;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BusyTimer _tHigh;
        public readonly object _lockObj = new object();

        public PriorityActionQueue()
        {
            _tLow = new BusyTimer(5000);
            _tLow.Elapsed += _tLow_Elapsed;
            _tHigh = new BusyTimer(10);
            _tHigh.Elapsed += _tHigh_Elapsed;

            _tLow.Start();
            _tHigh.Start();
        }

        private void _tLow_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                var a = DequeueBottomAction();
                if (a != null)
                    a.Invoke();
            }
        }

        private void _tHigh_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                var a = DequeueBottomAction(_qHigh);
                if (a != null)
                {
                    DequeueBottomAction(_qLow);
                    a.Invoke();
                }
            }
        }

        private Action DequeueBottomAction()
        {
            var a1 = DequeueBottomAction(_qLow);
            var a2 = DequeueBottomAction(_qHigh);
            if (a1 != null)
                return a1;
            return a2;
        }

        private static Action DequeueBottomAction(Queue<Action> q)
        {
            Action action = null;
            while (q.Count > 0)
                action = q.Dequeue();
            return action;
        }

        public void EnqueueLow(Action action)
        {
            lock (_lockObj)
                _qLow.Enqueue(action);
        }

        public void EnqueueHigh(Action action)
        {
            lock (_lockObj)
                _qHigh.Enqueue(action);
        }
    }
}