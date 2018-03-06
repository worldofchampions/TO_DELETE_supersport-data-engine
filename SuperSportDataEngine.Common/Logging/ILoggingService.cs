using System;
using System.Threading.Tasks;
using SuperSportDataEngine.Common.Interfaces;

namespace SuperSportDataEngine.Common.Logging
{
    public interface ILoggingService
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }

        ICache Cache { get; set; }

        Task Debug(string key, Exception exception);
        Task Debug(string key, string format, params object[] args);
        Task Debug(string key, Exception exception, string format, params object[] args);
        Task Error(string key, Exception exception);
        Task Error(string key, string format, params object[] args);
        Task Error(string key, Exception exception, string format, params object[] args);
        Task Fatal(string key, Exception exception);
        Task Fatal(string key, string format, params object[] args);
        Task Fatal(string key, Exception exception, string format, params object[] args);
        Task Info(string key, Exception exception);
        Task Info(string key, string format, params object[] args);
        Task Info(string key, Exception exception, string format, params object[] args);
        Task Trace(string key, Exception exception);
        Task Trace(string key, string format, params object[] args);
        Task Trace(string key, Exception exception, string format, params object[] args);
        Task Warn(string key, Exception exception);
        Task Warn(string key, string format, params object[] args);
        Task Warn(string key, Exception exception, string format, params object[] args);
    }
}
