using System;
using System.Threading;

namespace Lib.Base
{
//2015-03-16 16:36:45,281 9576   ReaderLock<TResult> ReleaseWriterLock start. 
//2015-03-16 16:36:45,281 9576   ReaderLock<TResult> ReleaseWriterLock end. 
//2015-03-16 16:36:45,281 9576   WriterLock start, <AddOrUpdate>b__11 
//2015-03-16 16:36:45,281 9576   WriterLock action start. 
//2015-03-16 16:36:45,285 9576   WriterLock start, <UpdateRange>b__14 
//2015-03-16 16:36:45,285 9576   WriterLock action start. 

//2015-03-16 16:36:45,306 9576   WriterLock action end.                         // try finally delay issuse - lijian
//2015-03-16 16:36:47,208 9576   WriterLock ReleaseWriterLock start. 

//2015-03-16 16:36:47,208 9576   WriterLock ReleaseWriterLock end. 
//2015-03-16 16:36:47,208 9576   WriterLock action end. 
//2015-03-16 16:36:47,208 9576   WriterLock ReleaseWriterLock start. 
//2015-03-16 16:36:47,208 9576   WriterLock ReleaseWriterLock end. 

    public abstract class ReaderWriterLockWrapper
    {
        private readonly ReaderWriterLock _rwl = new ReaderWriterLock();

        public TResult WriterLock<TResult>(Func<TResult> function)
        {
            try
            {
                Log("WriterLock<TResult> start, " + function.Method.Name);
                _rwl.AcquireWriterLock(int.MaxValue);
                try
                {
                    Log("WriterLock<TResult> function start.");
                    var ret = function();
                    Log("WriterLock<TResult> function end.");
                    return ret;
                }
                finally
                {
                    Log("WriterLock<TResult> ReleaseWriterLock start.");
                    _rwl.ReleaseWriterLock();
                    Log("WriterLock<TResult> ReleaseWriterLock end.");
                }
            }
            catch (ApplicationException ex)
            {
                Log("WriterLock<TResult> failed.", ex);
                return default(TResult);
            }
        }

        public TResult ReaderLock<TResult>(Func<TResult> function)
        {
            try
            {
                Log("ReaderLock<TResult> start, " + function.Method.Name);
                _rwl.AcquireReaderLock(int.MaxValue);
                try
                {
                    Log("ReaderLock<TResult> function start.");
                    var ret = function();
                    Log("ReaderLock<TResult> function end.");
                    return ret;
                }
                finally
                {
                    Log("ReaderLock<TResult> ReleaseWriterLock start.");
                    _rwl.ReleaseReaderLock();
                    Log("ReaderLock<TResult> ReleaseWriterLock end.");
                }
            }
            catch (ApplicationException ex)
            {
                Log("ReaderLock<TResult> failed.", ex);
                return default(TResult);
            }
        }

        public void WriterLock(Action action)
        {
            try
            {
                Log("WriterLock start, " + action.Method.Name);
                _rwl.AcquireWriterLock(int.MaxValue);
                try
                {
                    Log("WriterLock action start.");
                    action();
                    Log("WriterLock action end.");
                }
                finally
                {
                    Log("WriterLock ReleaseWriterLock start.");
                    _rwl.ReleaseWriterLock();
                    Log("WriterLock ReleaseWriterLock end.");
                }
            }
            catch (ApplicationException ex)
            {
                Log("WriterLock failed.", ex);
            }
        }

        public void ReaderLock(Action action)
        {
            try
            {
                Log("ReaderLock start, " + action.Method.Name);
                _rwl.AcquireReaderLock(int.MaxValue);
                try
                {
                    Log("ReaderLock action start.");
                    action();
                    Log("ReaderLock action end.");
                }
                finally
                {
                    Log("ReaderLock ReleaseWriterLock start.");
                    _rwl.ReleaseReaderLock();
                    Log("ReaderLock ReleaseWriterLock end.");
                }
            }
            catch (ApplicationException ex)
            {
                Log("ReaderLock failed.", ex);
            }
        }

        protected abstract void Log(string s, Exception ex = null);
    }
}