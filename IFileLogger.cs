using System;

namespace Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileLogger
    {
        event Action<object, LoggerEventArgs> LogEvent;
        void OnLogEvent(object o, LoggerEventArgs eventArgs);
        LoggerEventArgs GetEventArgs(string message, string className, string methodName, Exception e);
    }
}
