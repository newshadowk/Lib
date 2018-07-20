using System;
using System.IO;

namespace Lib.Base
{
    public class DoubleTaskTimerReset : IDisposable
    {
        #region Fields

        private readonly TaskTimerReset _t1;

        private readonly TaskTimerReset _t2;

        private volatile bool _elapsedBusy;

        private bool _t2NeedReset = true;

        public event EventHandler Elapsed;

        #endregion

        #region Constructors

        public DoubleTaskTimerReset(double oneDetectTime = 3000, double twoDetectTime = 5000)
        {
            _t1 = new TaskTimerReset(oneDetectTime);
            _t2 = new TaskTimerReset(twoDetectTime);

            _t1.Elapsed += _t1_Elapsed;
            _t2.Elapsed += _t2_Elapsed;
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
            _t1.ResetTime();
            if (_t2NeedReset)
            {
                _t2NeedReset = false;
                _t2.ResetTime();
            }
        }

        public void Stop()
        {
            _t1.Stop();
            _t2.Stop();
        }

        public void Close()
        {
            _t1.Close();
            _t2.Close();
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

        private void _t2_Elapsed(object sender, EventArgs e)
        {
            _t2NeedReset = true;
            InvokeElapsed();
        }

        private void _t1_Elapsed(object sender, EventArgs e)
        {
            _t2.Stop();
            _t2NeedReset = true;
            InvokeElapsed();
        }

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
            var handler = Elapsed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Others

        ~DoubleTaskTimerReset()
        {
            Dispose(false);
        }

        #endregion
    }
}