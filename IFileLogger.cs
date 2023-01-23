using System;
using System.Security.Cryptography.X509Certificates;

namespace QGJSoft.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileLogger
    {
        event Action<string, int, LoggerEventArgs> LogEvent;
        void OnLogEvent(string logFileFullPath, int maxLogFileSize, LoggerEventArgs eventArgs);
    }
}
