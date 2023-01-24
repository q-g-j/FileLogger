using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace QGJSoft.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileLogger
    {
        public static CancellationTokenSource CancellationTokenSource;

        private static readonly ConcurrentQueue<Tuple<string, int, LoggerEventArgs>> logQueue = new ConcurrentQueue<Tuple<string, int, LoggerEventArgs>>();

        static FileLogger()
        {
            CancellationTokenSource = new CancellationTokenSource();
            new Thread(() => LogWritingThread(CancellationTokenSource.Token)).Start();
        }

        public static void LogWriteLine(string logFileFullPath, int maxLogFileSizeInKB, LoggerEventArgs args)
        {
            logQueue.Enqueue(new Tuple<string, int, LoggerEventArgs>(logFileFullPath, maxLogFileSizeInKB, args));
        }

        private static void LogWritingThread(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (logQueue.TryDequeue(out Tuple<string, int, LoggerEventArgs> tuple))
                {
                    try
                    {
                        string message = DateTime.Now.ToString();

                        if (tuple.Item3.Exception != null)
                        {
                            string senderObject = tuple.Item3.SenderClass;
                            string senderMethod = tuple.Item3.SenderMethod;
                            string exceptionMessage = tuple.Item3.Exception.Message;
                            message += $" - Error in class {senderObject}, Method {senderMethod}: {exceptionMessage}";
                        }
                        else
                        {
                            message += $" - {tuple.Item3.Message}";
                        }

                        List<string> logFileContent = null;

                        if (File.Exists(tuple.Item1))
                        {
                            logFileContent = File.ReadAllLines(tuple.Item1, System.Text.Encoding.UTF8).ToList();
                        }
                        if (logFileContent == null)
                        {
                            logFileContent = new List<string>();
                        }

                        logFileContent.Add(message);

                        if (GetSizeOfStringListInBytes(logFileContent) > tuple.Item2 * 1024)
                        {
                            File.WriteAllLines(tuple.Item1, TrimToSizeInByte(logFileContent, tuple.Item2 * 1024), System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            File.WriteAllLines(tuple.Item1, logFileContent, System.Text.Encoding.UTF8);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Returns the size of a string list in Byte (= the size it would occupy on the disk). The size of the system's newline character is taken into account.
        /// </summary>
        /// <param name="stringList">A List of strings; to get it from a text file: File.ReadAllLines(fileName).ToList()</param>
        /// <returns>The size of the string list in Bytes.</returns>
        private static int GetSizeOfStringListInBytes(List<string> stringList)
        {
            int systemNewLineCharInBytes = System.Text.Encoding.UTF8.GetByteCount(Environment.NewLine);
            return System.Text.Encoding.UTF8.GetByteCount(string.Join("", stringList)) + systemNewLineCharInBytes * stringList.Count;
        }

        /// <summary>
        /// Trims a string list to a desired size in Byte (= the size it would occupy on the disk).
        /// </summary>
        /// <param name="stringList">A List of strings; to get it from a text file: File.ReadAllLines(fileName).ToList()</param>
        /// <param name="maxSizeInBytes">The desired maximal size of the string list in Bytes.</param>
        /// <returns>The trimmed string list.</returns>
        private static List<string> TrimToSizeInByte(List<string> stringList, int maxSizeInBytes)
        {
            int skip = stringList.Count / 2;
            List<string> trimmedList = new List<string>(stringList);

            while (skip > 0)
            {
                while (GetSizeOfStringListInBytes(trimmedList) > maxSizeInBytes)
                {
                    List<string> backupTrimmedList = new List<string>(trimmedList);
                    trimmedList = new List<string>(trimmedList.Skip(skip).ToList());
                    if (GetSizeOfStringListInBytes(trimmedList) < maxSizeInBytes)
                    {
                        trimmedList = backupTrimmedList;
                        break;
                    }
                }

                skip /= 2;
            }

            if (GetSizeOfStringListInBytes(trimmedList) > maxSizeInBytes)
            {
                trimmedList = trimmedList.Skip(1).ToList();
            }

            return trimmedList;
        }
    }
}
