using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Magician.Common.Util
{
    internal class ApplicationUtil
    {
        #region StartApplication

        /// <summary>
        ///     StartApplication 启动一个应用程序/进程
        /// </summary>
        public static void StartApplication(string appFilePath)
        {
            var process = new Process {StartInfo = {FileName = appFilePath}};
            process.Start();
        }

        #endregion

        #region OpenUrl

        /// <summary>
        ///     OpenUrl 在浏览器中打开wsUrl链接
        /// </summary>
        public static void OpenUrl(string url)
        {
            Process.Start(url);
        }

        #endregion

        #region IsAppInstanceExist

        /// <summary>
        ///     IsAppInstanceExist 目标应用程序是否已经启动。通常用于判断单实例应用。将占用锁。
        /// </summary>
        public static bool IsAppInstanceExist(string instanceName)
        {
            bool createdNew;
            var mutex = new Mutex(false, instanceName, out createdNew);
            if (createdNew) mutexForSingletonExeDic.Add(instanceName, mutex);

            return !createdNew;
        }

        private static readonly Dictionary<string, Mutex> mutexForSingletonExeDic = new Dictionary<string, Mutex>();

        /// <summary>
        ///     释放由IsAppInstanceExist占用的锁。
        /// </summary>
        public static void ReleaseAppInstance(string instanceName)
        {
            Mutex mutex = null;
            if (mutexForSingletonExeDic.ContainsKey(instanceName)) mutex = mutexForSingletonExeDic[instanceName];

            if (mutex != null)
            {
                mutex.Close();
                mutexForSingletonExeDic.Remove(instanceName);
            }
        }

        #endregion
    }
}