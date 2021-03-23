using System;

namespace Magician.Common.Logger
{
    /// <summary>
    ///     IAgileLogger 用于记录日志信息。log方法不会抛出异常！
    ///     通常可以通过ESFramework.Common.AdvancedFunction.SetProperty 方法来简化组件的日志记录器的装配。
    /// </summary>
    public interface IAgileLogger
    {
        bool Enabled { set; }
        event Action<Exception> InnerExceptionOccurred;

        void LogWithTime(string msg);
        void Log(string errorType, string msg, string location, ErrorLevel level);
        void Log(Exception ee, string location, ErrorLevel level);

        /// <summary>
        ///     LogSimple 不记录异常的堆栈位置
        /// </summary>
        void LogSimple(Exception ee, string location, ErrorLevel level);
    }

    #region EmptyAgileLogger

    public sealed class EmptyAgileLogger : IAgileLogger
    {
        #region ILogger 成员

        public void Log(string errorType, string msg, string location, ErrorLevel level)
        {
        }

        public bool Enabled
        {
            set
            {

            }
        }

        #endregion

        #region IAgileLogger 成员

        public event Action<Exception> InnerExceptionOccurred;

        public void Log(Exception ee, string location, ErrorLevel level)
        {
        }

        public void LogSimple(Exception ee, string location, ErrorLevel level)
        {
        }

        public void LogWithTime(string msg)
        {
        }

        #endregion
    }

    #endregion
}