using log4net;
using log4net.Appender;
using log4net.Layout;
using System;

namespace SimpleXAMLLocalizationHelper.Utils
{
    public static class Logger
    {
        public static readonly ILog LogWriterMessage = GetAppLoggerMessage();
        public static readonly ILog LogWriterException = GetAppLoggerException();

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        private static ILog GetAppLoggerMessage()
        {
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "3MB";
            roller.File = $@"logs\{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}.log";

            roller.StaticLogFileName = true;
            roller.Layout = new PatternLayout("%d{yyMMdd HH:mm:ss.fff} %-5p : %m%n");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            DummyLogger dummyILogger = new DummyLogger("AppLog");
            dummyILogger.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            dummyILogger.Level = log4net.Core.Level.Info;
            dummyILogger.AddAppender(roller);

            return new LogMe(dummyILogger);
        }

        private static ILog GetAppLoggerException()
        {
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "3MB";
            roller.File = $@"logs\{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}_ex.log";

            roller.StaticLogFileName = true;
            roller.Layout = new PatternLayout("%d{yyMMdd HH:mm:ss.fff} %-5p : %m%n");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            DummyLogger dummyILogger = new DummyLogger("AppLogEx");
            dummyILogger.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            dummyILogger.Level = log4net.Core.Level.Info;
            dummyILogger.AddAppender(roller);

            return new LogEx(dummyILogger);
        }
    }

    public sealed class DummyLogger : log4net.Repository.Hierarchy.Logger
    {
        // Methods
        internal DummyLogger(string name)
            : base(name)
        {
        }
    }

    public class LogMe : log4net.Core.LogImpl
    {
        public LogMe(DummyLogger log) : base(log)
        {
        }
    }

    public class LogEx : log4net.Core.LogImpl
    {
        public LogEx(DummyLogger log) : base(log)
        {
        }

        public override void Error(object message, Exception exception)
        {
            base.Error(exception);
        }

        public override void Warn(object message, Exception exception)
        {
            base.Warn(exception);
        }
    }
}