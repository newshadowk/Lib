using System.Timers;

namespace Lib.Base
{
    public class SmartSyncQueue<T> : SyncQueue<T>
    {
        private readonly Timer _t;

        public SmartSyncQueue(int trimExcessIntervalMs = 10000)
        {
            _t = new Timer(trimExcessIntervalMs);
            _t.Elapsed += T_Elapsed;
            _t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            TrimExcess();
        }
    }
}