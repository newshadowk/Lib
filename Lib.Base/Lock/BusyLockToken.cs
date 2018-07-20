using System;

namespace Lib.Base
{
    public class BusyLockToken : IDisposable
    {
        private readonly BusyLock _busyLock;

        public BusyLockToken(bool lockGoted, BusyLock busyLock)
        {
            LockGoted = lockGoted;
            _busyLock = busyLock;
        }

        public bool LockGoted { get; private set; }

        public void Dispose()
        {
            if (LockGoted)
                _busyLock.Release();
        }
    }
}
