using System;
using System.Timers;

namespace Lib.Base
{
    /// <summary>
    /// Thread-safe.
    /// </summary>
    public class TaskTimerReset : IDisposable
    {
        #region Fields

        private readonly object _lockObj = new object();
        private readonly Timer _timer = new Timer();

        private volatile bool _elapsedBusy;

        public event EventHandler Elapsed;

        #endregion

        #region Constructors

        public TaskTimerReset(double detectTime = 1000)
        {
            _timer.Interval = detectTime;
            _timer.Elapsed += Timer_Elapsed;
        }

        #endregion

        #region Methods public

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ResetTime()
        {
            lock (_lockObj)
            {
                _timer.Stop();
                _timer.Start();
            }
        }

        public void Stop()
        {
            lock (_lockObj)
                _timer.Stop();
        }

        public void Close()
        {
            lock (_lockObj)
            {
                _timer.Stop();
                _timer.Close();
                _timer.Dispose();
            }
        }

        #endregion

        #region Methods protected

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion

        #region Methods private

        private void InvokeElapsed()
        {
            if (_elapsedBusy)
            {
                ResetTime();
                return;
            }

            _elapsedBusy = true;
            OnElapsed();
            _elapsedBusy = false;
        }

        private void OnElapsed()
        {
            EventHandler handler = Elapsed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Stop();
            InvokeElapsed();
        }

        #endregion

        #region Others

        ~TaskTimerReset()
        {
            Dispose(false);
        }

        #endregion
    }
}