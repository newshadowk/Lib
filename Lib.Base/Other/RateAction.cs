using System;
using System.Threading.Tasks;

namespace Lib.Base
{
    public sealed class RateAction : IDisposable
    {
        public readonly BusyTimer T;
        private Action _action;
        private Action _lastAction;
        private bool _isEnd;
        private readonly object _lockObj = new object();

        public RateAction(int intervalMs)
        {
            T = new BusyTimer(intervalMs);
            T.Elapsed += TElapsed;
            T.Start();
        }

        private void TElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke();
        }

        private void Invoke()
        {
            lock (_lockObj)
            {
                if (_isEnd)
                    return;

                if (_action == _lastAction)
                    return;

                _lastAction = _action;
                _lastAction.Invoke();
            }
        }

        public void Post(Action action, bool force = false, bool isEnd = false)
        {
            lock (_lockObj)
            {
                if (_isEnd)
                    return;
                _action = action;
                _isEnd = isEnd;

                if (isEnd)
                {
                    action.Invoke();
                    return;
                }
            }

            if (force)
                Task.Run(() => Invoke());
        }

        public void Dispose()
        {
            T?.Dispose();
        }
    }
}