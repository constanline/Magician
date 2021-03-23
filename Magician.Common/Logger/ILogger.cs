using System;

namespace Magician.Common.Logger
{
    /// <summary>
    ///     ILogger 用于日志记录的基础接口，线程安全的
    /// </summary>
    public interface ILogger : IDisposable
    {
        bool Enabled { get; set; }
        void Log(string msg);
        void LogWithTime(string msg);
    }

    public class EmptyLogger : ILogger
    {
        #region Enabled

        public bool Enabled
        {
            get { return true; }
            set { }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
        }

        #endregion

        #region ILogger 成员

        #region Log

        public void Log(string msg)
        {
        }

        #endregion

        #region LogWithTime

        public void LogWithTime(string msg)
        {
        }

        #endregion

        #endregion
    }
}