using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class FileLogger
    {
        public FileLogger(string _logFileName, int _maxFileSizeInKB)
        {
            logFileName = _logFileName;
            maxFileSizeInKB = _maxFileSizeInKB;
        }

        private readonly string logFileName;
        private readonly int maxFileSizeInKB;

        public void LogWriteLine(object o, LoggerEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    string message = DateTime.Now.ToString();

                    if (args.Exception != null)
                    {
                        string senderObject = args.SenderClass;
                        string senderMethod = args.SenderMethod;
                        string exceptionMessage = args.Exception.Message;
                        message += $" - Error in class {senderObject}, Method {senderMethod}: {exceptionMessage}";
                    }
                    else
                    {
                        message += $" - {args.Message}";
                    }

                    List<string> logFileContent = null;

                    if (File.Exists(logFileName))
                    {
                        WaitForFile(logFileName);
                        logFileContent = File.ReadAllLines(logFileName, System.Text.Encoding.ASCII).ToList();
                    }
                    if (logFileContent == null)
                    {
                        logFileContent = new List<string>();
                    }

                    if (GetSizeOfStringListInBytes(logFileContent) > maxFileSizeInKB * 1024)
                    {
                        WaitForFile(logFileName);
                        File.WriteAllLines(logFileName, TrimToSizeInByte(logFileContent, maxFileSizeInKB * 1024), System.Text.Encoding.ASCII);
                    }
                    else
                    {
                        WaitForFile(logFileName);
                        File.WriteAllLines(logFileName, logFileContent, System.Text.Encoding.ASCII);
                    }
                }
                catch
                {
                }
            });
        }

        /// <summary>
        /// Returns the size of a string list in Byte (= the size it would occupy on the disk). The size of the system's newline character is taken into account.
        /// </summary>
        /// <param name="stringList">A List of strings; to get it from a text file: File.ReadAllLines(fileName).ToList()</param>
        /// <returns>The size of the string list in Bytes.</returns>
        private static int GetSizeOfStringListInBytes(List<string> stringList)
        {
            int systemNewLineCharInBytes = System.Text.Encoding.ASCII.GetByteCount(Environment.NewLine);
            return System.Text.Encoding.ASCII.GetByteCount(string.Join("", stringList)) + systemNewLineCharInBytes * stringList.Count;
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

        private static bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void WaitForFile(string filename)
        {
            int counter = 0;
            while (!IsFileReady(filename) && counter < 1000)
            {
                counter++;
            }
        }
    }
}
