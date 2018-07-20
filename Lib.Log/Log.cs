using System;
using System.Linq;
using NLog;

namespace Lib.Log
{
    public static class Log
    {
        private const string DefaultModuleName = "log";

        public static bool IsEnabled { get; set; }

        public static void Init(string savePath = null, params string[] moduleNames)
        {
            var list = moduleNames.ToList();
            list.Add(DefaultModuleName);
            NLogProxy.Init(savePath, list);
            foreach (var m in moduleNames)
                Info(m, "----------- [Start] -----------");
        }

        public static void InitModules(params string[] moduleNames)
        {
            Init(null, moduleNames);
        }

        public static void Write(string moduleName, LogLevel level, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, level, msg, ex);
        }

        public static void Write(LogLevel level, string msg, Exception ex = null)
        {
            NLogProxy.Write(DefaultModuleName, level, msg, ex);
        }

        public static void Trace(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Trace, msg, ex);
        }

        public static void Trace(string msg, Exception ex = null)
        {
            Trace(DefaultModuleName, msg, ex);
        }

        public static void Debug(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Debug, msg, ex);
        }

        public static void Debug(string msg, Exception ex = null)
        {
            Debug(DefaultModuleName, msg, ex);
        }

        public static void Info(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Info, msg, ex);
        }

        public static void Info(string msg, Exception ex = null)
        {
            Info(DefaultModuleName, msg, ex);
        }

        public static void Warn(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Warn, msg, ex);
        }

        public static void Warn(string msg, Exception ex = null)
        {
            Warn(DefaultModuleName, msg, ex);
        }

        public static void Error(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Error, msg, ex);
        }

        public static void Error(string msg, Exception ex = null)
        {
            Error(DefaultModuleName, msg, ex);
        }

        public static void Fatal(string moduleName, string msg, Exception ex = null)
        {
            NLogProxy.Write(moduleName, LogLevel.Fatal, msg, ex);
        }

        public static void Fatal(string msg, Exception ex = null)
        {
            Fatal(DefaultModuleName, msg, ex);
        }

        public static string GetString(string msg, Exception ex = null)
        {
            return $"{msg} {NLogProxy.GetException(ex)}";
        }
    }
}