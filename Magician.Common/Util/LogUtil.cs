using System;
using System.IO;

namespace Magician.Common.Util
{
    internal class LogUtil
    {
        private static readonly object logLock = string.Empty;
        public static void Log(string message)
        {
            lock (logLock)
            {
                if (!Directory.Exists("log")) Directory.CreateDirectory("log");
                var filePath = "log/" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
                var fs = !File.Exists(filePath) ? File.Create(filePath) : new FileStream(filePath, FileMode.Append, FileAccess.Write);
                var sw = new StreamWriter(fs);
                sw.WriteLine(message);
                sw.Close();
                fs.Close();
            }
        }
    }
}