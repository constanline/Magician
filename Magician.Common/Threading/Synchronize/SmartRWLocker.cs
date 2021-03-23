using System;
using System.Threading;

namespace Magician.Common.Threading.Synchronize
{
    /// <summary>
    ///     SmartRWLocker 简化了ReaderWriterLock的使用。通过using来使用Lock方法返回的对象，如：using(this.smartLocker.Lock(AccessMode.Read)){...}
    ///     zhuweisky 2008.11.25
    /// </summary>
    public class SmartRwLocker
    {
        private readonly ReaderWriterLock _readerWriterLock = new ReaderWriterLock();

        #region LastRequireReadTime

        public DateTime LastRequireReadTime { get; private set; } = DateTime.Now;

        #endregion

        #region LastRequireWriteTime

        public DateTime LastRequireWriteTime { get; private set; } = DateTime.Now;

        #endregion

        #region Lock

        public LockingObject Lock(AccessMode accessMode, bool enableSynchronize)
        {
            if (!enableSynchronize) return null;

            return Lock(accessMode);
        }

        public LockingObject Lock(AccessMode accessMode)
        {
            if (accessMode == AccessMode.Read)
                LastRequireReadTime = DateTime.Now;
            else
                LastRequireWriteTime = DateTime.Now;

            return new LockingObject(_readerWriterLock, accessMode);
        }

        #endregion
    }

    /// <summary>
    ///     AccessMode 访问锁定资源的方式。
    /// </summary>
    public enum AccessMode
    {
        Read = 0,
        Write,

        /// <summary>
        ///     前提条件：已经获取Read锁。
        ///     再采用此模式，可以先升级到Write，访问资源，再降级回Read。
        /// </summary>
        UpAndDowngrade4Write
    }
}