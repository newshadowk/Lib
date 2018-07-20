using System;
using System.Threading;

namespace Lib.Base
{
    public static class ReaderWriterLockHelper
    {
        public static TResult WriterLock<TResult>(Func<TResult> function, ReaderWriterLock rwl)
        {
            try
            {
                rwl.AcquireWriterLock(int.MaxValue);
                try
                {
                    return function();
                }
                finally
                {
                    rwl.ReleaseWriterLock();
                }
            }
            catch (ApplicationException)
            {
                return default(TResult);
            }
        }

        public static TResult ReaderLock<TResult>(Func<TResult> function, ReaderWriterLock rwl)
        {
            try
            {
                rwl.AcquireReaderLock(int.MaxValue);
                try
                {
                    return function();
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                return default(TResult);
            }
        }

        public static void WriterLock(Action action, ReaderWriterLock rwl)
        {
            try
            {
                rwl.AcquireWriterLock(int.MaxValue);
                try
                {
                    action();
                }
                finally
                {
                    rwl.ReleaseWriterLock();
                }
            }
            catch (ApplicationException)
            {
            }
        }

        public static void ReaderLock(Action action, ReaderWriterLock rwl)
        {
            try
            {
                rwl.AcquireReaderLock(int.MaxValue);
                try
                {
                    action();
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
            }
        }
    }
}