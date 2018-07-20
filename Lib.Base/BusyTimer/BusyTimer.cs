using System;
using System.Timers;

namespace Lib.Base
{
    public class BusyTimer : IDisposable
    {
        private readonly Timer T = new Timer();
        private readonly BusyLock TLock = new BusyLock();

        public BusyTimer(double interval)
        {
            T.Elapsed += T_Elapsed;
            T.Interval = interval;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            T.Dispose();
        }

        public void Start(bool isImmediately = false)
        {
            Log("Start, start.");
            TryStart();
            if (isImmediately)
                DoElapsed(null);
            Log("Start, end.");
        }

        public void Stop()
        {
            Log("Stop, start");
            TryStop();
            Log("Stop, end.");
        }

        public event ElapsedEventHandler Elapsed;

        private void TryStart()
        {
            //Possible T is already Disposed.
            try
            {
                T.Start();
            }
            catch
            {
            }
        }

        private void TryStop()
        {
            //Possible T is already Disposed.
            try
            {
                T.Stop();
            }
            catch
            {
            }
        }

        private void OnElapsed(ElapsedEventArgs e)
        {
            Elapsed?.Invoke(this, e);
        }

        private void DoElapsed(ElapsedEventArgs e)
        {
            using (var token = TLock.Lock())
            {
                if (!token.LockGoted)
                {
                    Log("DoElapsed, return.");
                    return;
                }
                TryStop();
                OnElapsed(e);
                TryStart();
            }
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Log("T_Elapsed, start.");
            DoElapsed(e);
            Log("T_Elapsed, stop.");
        }

        protected virtual void Log(string msg, Exception ex = null)
        {
        }

        ~BusyTimer()
        {
            Dispose(false);
        }
    }
}