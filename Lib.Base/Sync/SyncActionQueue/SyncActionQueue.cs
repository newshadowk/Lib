using System;
using System.Timers;

namespace Lib.Base
{
    public class SyncActionQueue
    {
        private readonly SmartSyncQueue<Action> _q = new SmartSyncQueue<Action>();

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BusyTimer _t;

        public SyncActionQueue(int checkIntervalMs)
        {
            _t = new BusyTimer(checkIntervalMs);
            _t.Elapsed += _t_Elapsed;
            _t.Start();
        }

        public int Count
        {
            get { return _q.Count; }
        }

        private void _t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Action result;
            while (_q.TryDequeue(out result))
            {
                try
                {
                    Log("[↑] " + Count);
                    result.Invoke();
                    Log("Invoke ok.");
                }
                catch (Exception ex)
                {
                    Log("Invoke failed.", ex);
                    throw;
                }
            }
        }

        public void Enqueue(Action action)
        {
            Log("[↓] " + Count);
            _q.Enqueue(action);
        }

        public virtual void Log(string msg, Exception ex = null)
        {
        }
    }
}