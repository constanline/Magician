using System;
using System.Threading;

namespace Magician.Common.Threading.Synchronize
{
    /// <summary>
    ///     LockingObject SmartRWLocker的Lock方法返回的锁对象。仅仅通过using来使用该对象，如：using(this.smartLocker.Lock(AccessMode.Read)){...}
    /// </summary>
    public class LockingObject : IDisposable
    {
        private readonly AccessMode _accessMode;
        private LockCookie _lockCookie;
        private readonly ReaderWriterLock _rwLock;

        #region Ctor

        public LockingObject(ReaderWriterLock rwLock, AccessMode lockMode)
        {
            _rwLock = rwLock;
            _accessMode = lockMode;

            if (_accessMode == AccessMode.Read)
                _rwLock.AcquireReaderLock(-1);
            else if (_accessMode == AccessMode.Write)
                _rwLock.AcquireWriterLock(-1);
            else //UpAndDowngrade
                _lockCookie = _rwLock.UpgradeToWriterLock(-1);
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (_accessMode == AccessMode.Read)
                _rwLock.ReleaseReaderLock();
            else if (_accessMode == AccessMode.Write)
                _rwLock.ReleaseWriterLock();
            else //UpAndDowngrade
                _rwLock.DowngradeFromWriterLock(ref _lockCookie);
        }

        #endregion
    }
}