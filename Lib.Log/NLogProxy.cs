using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Lib.Log
{
    //Trace    0
    //Debug    1
    //Info     2
    //Warn     3
    //Error    4
    //Fatal    5
    internal static class NLogProxy
    {
        private static string _savePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\logs";

        private static int _mainThreadId = -1;

        private static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();

        public static void Init(string savePath, List<string> moduleNames)
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!string.IsNullOrEmpty(savePath))
                _savePath = savePath;
            InitLoggers(moduleNames.ToList());
        }

        private static void InitLoggers(List<string> moduleNames)
        {
            var config = new LoggingConfiguration();
            foreach (var moduleName in moduleNames)
                InitConfig(moduleName, config);

            LogManager.Configuration = config;

            foreach (var moduleName in moduleNames)
            {
                var logger = LogManager.GetLogger(moduleName);
                Loggers.Add(moduleName, logger);
            }
        }

        private static void InitConfig(string moduleName, LoggingConfiguration config)
        {
            var fileTarget = GetFileTarget(moduleName);
            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, moduleName, moduleName);
        }

        public static void Write(string moduleName, LogLevel logLevel, string msg, Exception ex = null)
        {
            Log(Loggers[moduleName], logLevel, GetMsg(logLevel, msg, ex));
        }

        private static FileTarget GetFileTarget(string moduleName)
        {
            var fileTarget = new FileTarget();
            fileTarget.Name = moduleName;
            fileTarget.FileName = $"{_savePath}\\{moduleName}.txt";
            fileTarget.Layout = "${message}";
            fileTarget.ArchiveAboveSize = 64 * 1024 * 1024;
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.DateAndSequence;
            fileTarget.MaxArchiveFiles = 10;
            return fileTarget;
        }

        private static string GetMsg(LogLevel logLevel, string msg, Exception ex)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string level = logLevel.Name.Substring(0, 1);
            string tId = GetThreadId();
            return $"{date} {level} {tId} {msg}{GetException(ex)}";
        }

        private static string GetThreadId()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            if (id == _mainThreadId)
                return (id + "*").PadRight(5);
            return id.ToString().PadRight(5);
        }

        public static string GetException(Exception ee)
        {
            if (ee == null)
                return " ";

            //10 layers InnerException.
            var msgContent = new StringBuilder("\r\n\r\n[Exception]\r\n");
            msgContent.Append(GetMsgContent(ee));
            if (ee.InnerException != null)
            {
                msgContent.Append("\r\n[InnerException]\r\n");
                msgContent.Append(GetMsgContent(ee.InnerException));
                if (ee.InnerException.InnerException != null)
                {
                    msgContent.Append("\r\n[InnerException]\r\n");
                    msgContent.Append(GetMsgContent(ee.InnerException.InnerException));
                    if (ee.InnerException.InnerException.InnerException != null)
                    {
                        msgContent.Append("\r\n[InnerException]\r\n");
                        msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException));
                        if (ee.InnerException.InnerException.InnerException.InnerException != null)
                        {
                            msgContent.Append("\r\n[InnerException]\r\n");
                            msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException));
                            if (ee.InnerException.InnerException.InnerException.InnerException.InnerException != null)
                            {
                                msgContent.Append("\r\n[InnerException]\r\n");
                                msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException.InnerException));
                                if (ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException != null)
                                {
                                    msgContent.Append("\r\n[InnerException]\r\n");
                                    msgContent.Append(
                                        GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException));
                                    if (ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException != null)
                                    {
                                        msgContent.Append("\r\n[InnerException]\r\n");
                                        msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException.InnerException
                                            .InnerException.InnerException));
                                        if (ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException
                                                .InnerException != null)
                                        {
                                            msgContent.Append("\r\n[InnerException]\r\n");
                                            msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException.InnerException
                                                .InnerException.InnerException.InnerException));
                                            if (ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException
                                                    .InnerException.InnerException != null)
                                            {
                                                msgContent.Append("\r\n[InnerException]\r\n");
                                                msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException.InnerException
                                                    .InnerException.InnerException.InnerException.InnerException));
                                                if (ee.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException.InnerException
                                                        .InnerException.InnerException.InnerException != null)
                                                {
                                                    msgContent.Append("\r\n[InnerException]\r\n");
                                                    msgContent.Append(GetMsgContent(ee.InnerException.InnerException.InnerException.InnerException
                                                        .InnerException.InnerException.InnerException.InnerException.InnerException.InnerException));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return msgContent.ToString();
        }

        private static string GetMsgContent(Exception ee)
        {
            string ret = ee.Message;
            if (!string.IsNullOrEmpty(ee.StackTrace))
                ret += "\r\n" + ee.StackTrace;
            ret += "\r\n";
            return ret;
        }

        private static void Log(Logger log, LogLevel logLevel, string msg)
        {
            if (logLevel == LogLevel.Trace)
                log.Trace(msg);
            else if (logLevel == LogLevel.Debug)
                log.Debug(msg);
            else if (logLevel == LogLevel.Info)
                log.Info(msg);
            else if (logLevel == LogLevel.Error)
                log.Error(msg);
            else if (logLevel == LogLevel.Fatal)
                log.Fatal(msg);
            else if (logLevel == LogLevel.Warn)
                log.Warn(msg);
            else
                throw new ArgumentOutOfRangeException("logLevel");
        }
    }
}