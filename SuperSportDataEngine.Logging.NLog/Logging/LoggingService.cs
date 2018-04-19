using NLog;
using NLog.Config;
using SuperSportDataEngine.Common.Logging;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog.Internal;
using SuperSportDataEngine.Common.Interfaces;

namespace SuperSportDataEngine.Logging.NLog.Logging
{
    public class LoggingService : Logger, ILoggingService
    {
        private const string LoggerName = "NLogLogger";
        private static int _cacheTtlInMinutes = 15;

        public static ILoggingService GetLoggingService()
        {
            ConfigurationItemFactory.Default.LayoutRenderers
                .RegisterDefinition("utc_date", typeof(UtcDateRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers
                .RegisterDefinition("web_variables", typeof(WebVariablesRenderer));
            ILoggingService logger = (ILoggingService)LogManager.GetLogger(LoggerName, typeof(LoggingService));

            _cacheTtlInMinutes = int.Parse(new ConfigurationManager().AppSettings["loggingTtlInMinutes"]);

            return logger;
        }

        public async Task Debug(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Debug, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Debug(string key, string format, Exception exception, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Debug, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Debug(string key, Exception exception, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Debug, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        private async Task LogWithCache(string key, LogEventInfo logEvent)
        {
            // Add a namespace to the Redis key.
            //key = key.Replace(":", "");
            key = $"LoggingService:{key}";

            if (Cache == null)
            {
                WriteLog(typeof(LoggingService), logEvent);
            }
            else
            {
                try
                {
                    object cacheObject = await Cache.GetAsync<LogEventInfo>(key);
                    if (cacheObject == null)
                    {
                        Cache.Add(key, logEvent, TimeSpan.FromMinutes(_cacheTtlInMinutes));
                        WriteLog(typeof(LoggingService), logEvent);
                    }
                }
                catch (Exception)
                {
                    // Ignore the exception and log it anyways?
                    WriteLog(typeof(LoggingService), logEvent);
                }
            }
        }

        private void WriteLog(Type type, LogEventInfo logEvent)
        {
            try
            {
                logEvent.Message =
                    Environment.MachineName +
                    Environment.NewLine +
                    logEvent.Message;
            }
            catch (Exception)
            {
                // ignored
            }

            Log(typeof(LoggingService), logEvent);
        }

        public async Task Error(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Error, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Error(string key, Exception exception, string format, params object[] args)
        {
            if (!IsErrorEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Error, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Fatal(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Fatal, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Fatal(string key, Exception exception, string format, params object[] args)
        {
            if (!IsFatalEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Fatal, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Info(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Info, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Info(string key, Exception exception, string format, params object[] args)
        {
            if (!IsInfoEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Info, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Trace(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Trace, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Trace(string key, Exception exception, string format, params object[] args)
        {
            if (!IsTraceEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Trace, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Warn(string key, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Warn, null, format, args);
            await LogWithCache(key, logEvent);
        }

        public async Task Warn(string key, Exception exception, string format, params object[] args)
        {
            if (!IsWarnEnabled) return;
            var logEvent = GetLogEvent(LoggerName, LogLevel.Warn, exception, format, args);
            await LogWithCache(key, logEvent);
        }

        public ICache Cache { get; set; }

        public async Task Debug(string key, Exception exception)
        {
            await Debug(key, exception, string.Empty);
        }

        public async Task Error(string key, Exception exception)
        {
            await Error(key, exception, string.Empty);
        }

        public async Task Fatal(string key, Exception exception)
        {
            await Fatal(key, exception, string.Empty);
        }

        public async Task Info(string key, Exception exception)
        {
            await Info(key, exception, string.Empty);
        }

        public async Task Trace(string key, Exception exception)
        {
            await Trace(key, exception, string.Empty);
        }

        public async Task Warn(string key, Exception exception)
        {
            await Warn(key, exception, string.Empty);
        }

        private LogEventInfo GetLogEvent(string loggerName, LogLevel level, Exception exception, string format, object[] args)
        {
            string assemblyProp = string.Empty;
            string classProp = string.Empty;
            string methodProp = string.Empty;
            string messageProp = string.Empty;
            string innerMessageProp = string.Empty;

            var logEvent = new LogEventInfo
                (level, loggerName, string.Format(format, args));

            if (exception != null)
            {
                assemblyProp = exception.Source;
                classProp = exception.TargetSite.DeclaringType.FullName;
                methodProp = exception.TargetSite.Name;
                messageProp = exception.Message;

                if (exception.InnerException != null)
                {
                    innerMessageProp = exception.InnerException.Message;
                }
            }

            logEvent.Properties["error-source"] = assemblyProp;
            logEvent.Properties["error-class"] = classProp;
            logEvent.Properties["error-method"] = methodProp;
            logEvent.Properties["error-message"] = messageProp;
            logEvent.Properties["inner-error-message"] = innerMessageProp;

            return logEvent;
        }
    }
}
