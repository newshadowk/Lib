using System;
using Microsoft.Extensions.Logging;

namespace Lib.Log
{
    public class NLogger : ILogger
    {
        private readonly string _categoryName;

        public NLogger(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
                categoryName = categoryName.Substring(categoryName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var msg = $"{_categoryName} {Format(state, exception)}";
            Lib.Log.Log.Write(Convert(logLevel), msg);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private static NLog.LogLevel Convert(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Information:
                    return NLog.LogLevel.Info;
                case LogLevel.Warning:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Critical:
                    return NLog.LogLevel.Fatal;
                case LogLevel.None:
                    return NLog.LogLevel.Off;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private static string Format<TState>(TState state, Exception e)
        {
            string s = $"{state} {NLogProxy.GetException(e)}";
            return s;
        }
    }
}