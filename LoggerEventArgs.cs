using System;

namespace Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggerEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string Message { get; set; }
        public string SenderClass { get; set; }
        public string SenderMethod { get; set; }
        public Exception Exception { get; set; }

        public LoggerEventArgs(string fileName,string message, string senderClass, string senderMethod, Exception exception)
        {
            FileName = fileName;
            Message = message;
            SenderClass = senderClass;
            SenderMethod = senderMethod;
            Exception = exception;
        }
    }
}
