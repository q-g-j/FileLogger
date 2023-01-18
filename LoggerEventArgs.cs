using System;

namespace QGJSoft.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggerEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string SenderClass { get; set; }
        public string SenderMethod { get; set; }
        public Exception Exception { get; set; }

        public LoggerEventArgs(string message, string senderClass, string senderMethod, Exception exception)
        {
            Message = message;
            SenderClass = senderClass;
            SenderMethod = senderMethod;
            Exception = exception;
        }
    }
}
