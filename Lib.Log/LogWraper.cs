using System;

namespace Lib.Log
{
    public class LogWraper
    {
        private readonly string _moduleName;

        private readonly bool _enabled;
        public string MsgPrefix { get; }

        public LogWraper(string moduleName, bool enabled = true, string msgPrefix = null)
        {
            _moduleName = moduleName;
            _enabled = enabled;
            MsgPrefix = msgPrefix ?? "";
        }

        public void Trace(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Trace(_moduleName, MsgPrefix + msg, ex);
        }

        public void Trace(Exception ex = null)
        {
            Trace(null, ex);
        }

        public void Debug(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Debug(_moduleName, MsgPrefix + msg, ex);
        }

        public void Debug(Exception ex = null)
        {
            Debug(null, ex);
        }

        public void Info(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Info(_moduleName, MsgPrefix + msg, ex);
        }

        public void Info(Exception ex = null)
        {
            Info(null, ex);
        }

        public void Warn(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Warn(_moduleName, MsgPrefix + msg, ex);
        }

        public void Warn(Exception ex = null)
        {
            Warn(null, ex);
        }

        public void Error(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Error(_moduleName, MsgPrefix + msg, ex);
        }

        public void Error(Exception ex = null)
        {
            Error(null, ex);
        }

        public void Fatal(string msg, Exception ex = null)
        {
            if (_enabled)
                Log.Fatal(_moduleName, MsgPrefix + msg, ex);
        }

        public void Fatal(Exception ex = null)
        {
            Fatal(null, ex);
        }
    }
}