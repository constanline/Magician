using System;
using System.Globalization;
using System.IO;
using Magician.Common.Core;
using Magician.Common.Util;

namespace Magician.Common.Logger
{
    /// <summary>
    ///     FileLogger 将日志记录到文本文件。FileLogger是线程安全的。
    /// </summary>
    public class FileLogger : ILogger
    {
        private StreamWriter _writer;
        private readonly string _iniPath;

        #region IDisposable 成员

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region MaxLength

        private long _maxLength = long.MaxValue;

        /// <summary>
        ///     当日志文件增加到一定的大小时，将创建一个新的文件记录日志。
        /// </summary>
        public long MaxLength4ChangeFile
        {
            get { return _maxLength; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException($"MaxLength4ChangeFile must greater than 0.");
                _maxLength = value;
            }
        }

        #endregion

        #region Ctor

        public FileLogger(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var fs = File.Create(filePath);
                fs.Close();
            }

            _iniPath = filePath;
            _writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate.Add(FileMode.Append), FileAccess.Write,
                FileShare.Read));
        }

        ~FileLogger()
        {
            Close();
        }

        #endregion

        #region ILogger 成员

        #region Log

        public void Log(string msg)
        {
            if (!Enabled) return;

            lock (_writer)
            {
                _writer.WriteLine(msg + "\n");
                _writer.Flush();
                CheckAndChangeNewFile();
            }
        }

        private void CheckAndChangeNewFile()
        {
            if (_writer.BaseStream.Length >= _maxLength)
            {
                _writer.Close();
                _writer = null;

                var fileName = FileUtil.GetFileNameNoPath(_iniPath);
                var dir = FileUtil.GetFileDirectory(_iniPath);
                var pos = fileName.LastIndexOf('.');
                string extendName = null;
                var pureName = fileName;
                if (pos >= 0)
                {
                    extendName = fileName.Substring(pos + 1);
                    pureName = fileName.Substring(0, pos);
                }

                string newPath = null;
                var i = 0;
                while (newPath == null)
                {
                    var newName = pureName + "_" + i++.ToString("000");
                    if (extendName != null) newName += "." + extendName;
                    var tryPath = dir + "\\" + newName;
                    if (!File.Exists(tryPath))
                    {
                        newPath = tryPath;
                    }
                }

                _writer = new StreamWriter(File.Open(newPath, FileMode.OpenOrCreate.Add(FileMode.Append), FileAccess.Write,
                    FileShare.Read));
            }
        }

        #endregion

        #region LogWithTime

        public void LogWithTime(string msg)
        {
            var formatMsg = string.Format("{0} : {1}", DateTime.Now.ToString(CultureInfo.CurrentCulture), msg);
            Log(formatMsg);
        }

        #endregion

        #region Close

        private void Close()
        {
            if (_writer != null)
                try
                {
                    _writer.Close();
                    _writer = null;
                }
                catch
                {
                    // ignored
                }
        }

        #endregion

        #region Enabled

        public bool Enabled { get; set; } = true;

        #endregion

        #endregion
    }
}