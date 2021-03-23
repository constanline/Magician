using System;
using Magician.Common.Core;
using Magician.Common.Logger;

namespace Magician.Common.Util
{
    /// <summary>
    ///     EventUtil 只有当事件的声明是以Action以及其泛型类型为委托类型时，才可以使用EventUtil来安全触发事件。
    /// </summary>
    public static class EventUtil
    {
        #region SpringEventSafely

        public static void SpringEventSafelyAsyn(IAgileLogger agileLogger, string eventPath, Delegate theEvent)
        {
            if (theEvent == null) return;

            var cb = new Action<IAgileLogger, string, Delegate>(SpringEventSafely);
            cb.BeginInvoke(agileLogger, eventPath, theEvent, null, null);
        }

        public static void SpringEventSafely(IAgileLogger agileLogger, string eventPath, Delegate theEvent)
        {
            if (theEvent == null) return;

            foreach (var invocation in theEvent.GetInvocationList())
                try
                {
                    var cb = (Action) invocation;
                    cb();
                }
                catch (Exception ee)
                {
                    while (ee.InnerException != null) ee = ee.InnerException;
                    agileLogger.Log(ee,
                        string.Format("{0} On handle event [{1}].", ReflectionUtil.GetMethodFullName(invocation.Method),
                            eventPath), ErrorLevel.Standard);
                }
        }

        #endregion

        #region SpringEventSafely<T1>

        public static void SpringEventSafelyAsyn<T1>(IAgileLogger agileLogger, string eventPath, Delegate theEvent,
            T1 t1)
        {
            if (theEvent == null) return;

            var cb = new Action<IAgileLogger, string, Delegate, T1>(SpringEventSafely);
            cb.BeginInvoke(agileLogger, eventPath, theEvent, t1, null, null);
        }

        public static void SpringEventSafely<T1>(IAgileLogger agileLogger, string eventPath, Delegate theEvent, T1 t1)
        {
            if (theEvent == null) return;

            foreach (var invocation in theEvent.GetInvocationList())
                try
                {
                    var cb = (Action<T1>) invocation;
                    cb(t1);
                }
                catch (Exception ee)
                {
                    while (ee.InnerException != null) ee = ee.InnerException;
                    agileLogger.Log(ee,
                        string.Format("{0} On handle event [{1}].", ReflectionUtil.GetMethodFullName(invocation.Method),
                            eventPath), ErrorLevel.Standard);
                }
        }

        #endregion

        #region SpringEventSafely<T1, T2>

        public static void SpringEventSafelyAsyn<T1, T2>(IAgileLogger agileLogger, string eventPath, Delegate theEvent,
            T1 t1, T2 t2)
        {
            if (theEvent == null) return;

            var cb = new Action<IAgileLogger, string, Delegate, T1, T2>(SpringEventSafely);
            cb.BeginInvoke(agileLogger, eventPath, theEvent, t1, t2, null, null);
        }

        public static void SpringEventSafely<T1, T2>(IAgileLogger agileLogger, string eventPath, Delegate theEvent,
            T1 t1, T2 t2)
        {
            if (theEvent == null) return;

            foreach (var invocation in theEvent.GetInvocationList())
                try
                {
                    var cb = (Action<T1, T2>) invocation;
                    cb(t1, t2);
                }
                catch (Exception ee)
                {
                    while (ee.InnerException != null) ee = ee.InnerException;
                    agileLogger.Log(ee,
                        string.Format("{0} On handle event [{1}].", ReflectionUtil.GetMethodFullName(invocation.Method),
                            eventPath), ErrorLevel.Standard);
                }
        }

        #endregion

        #region SpringEventSafelyAsyn<T1, T2, T3>

        public static void SpringEventSafelyAsyn<T1, T2, T3>(IAgileLogger agileLogger, string eventPath,
            Delegate theEvent, T1 t1, T2 t2, T3 t3)
        {
            if (theEvent == null) return;

            var cb = new Action<IAgileLogger, string, Delegate, T1, T2, T3>(SpringEventSafely);
            cb.BeginInvoke(agileLogger, eventPath, theEvent, t1, t2, t3, null, null);
        }

        public static void SpringEventSafely<T1, T2, T3>(IAgileLogger agileLogger, string eventPath, Delegate theEvent,
            T1 t1, T2 t2, T3 t3)
        {
            if (theEvent == null) return;

            foreach (var invocation in theEvent.GetInvocationList())
                try
                {
                    var cb = (Action<T1, T2, T3>) invocation;
                    cb(t1, t2, t3);
                }
                catch (Exception ee)
                {
                    while (ee.InnerException != null) ee = ee.InnerException;
                    agileLogger.Log(ee,
                        string.Format("{0} On handle event [{1}].", ReflectionUtil.GetMethodFullName(invocation.Method),
                            eventPath), ErrorLevel.Standard);
                }
        }

        #endregion

        #region SpringEventSafely<T1, T2, T3 ,T4>

        public static void SpringEventSafelyAsyn<T1, T2, T3, T4>(IAgileLogger agileLogger, string eventPath,
            Delegate theEvent, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (theEvent == null) return;

            var cb = new Action<IAgileLogger, string, Delegate, T1, T2, T3, T4>(SpringEventSafely);
            cb.BeginInvoke(agileLogger, eventPath, theEvent, t1, t2, t3, t4, null, null);
        }

        public static void SpringEventSafely<T1, T2, T3, T4>(IAgileLogger agileLogger, string eventPath,
            Delegate theEvent, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (theEvent == null) return;

            foreach (var invocation in theEvent.GetInvocationList())
                try
                {
                    var cb = (Action<T1, T2, T3, T4>) invocation;
                    cb(t1, t2, t3, t4);
                }
                catch (Exception ee)
                {
                    while (ee.InnerException != null) ee = ee.InnerException;
                    agileLogger.Log(ee,
                        string.Format("{0} On handle event [{1}].", ReflectionUtil.GetMethodFullName(invocation.Method),
                            eventPath), ErrorLevel.Standard);
                }
        }

        #endregion
    }

    /// <summary>
    ///     只有当事件的声明是以CbGeneric以及其泛型类型为委托类型时，才可以使用EventSafeTrigger来安全触发事件。
    /// </summary>
    public class EventSafeTrigger
    {
        #region AgileLogger

        private IAgileLogger _agileLogger = new EmptyAgileLogger();

        public IAgileLogger AgileLogger
        {
            set { _agileLogger = value; }
        }

        #endregion

        #region PublisherFullName

        public string PublisherFullName { get; set; } = "";

        #endregion

        #region Ctor

        public EventSafeTrigger()
        {
        }

        public EventSafeTrigger(IAgileLogger logger, string publisherTypeFullName)
        {
            _agileLogger = logger;
            PublisherFullName = publisherTypeFullName;
        }

        #endregion

        #region Action

        public void ActionAsyn(string eventName, Delegate theEvent)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafelyAsyn(_agileLogger, eventPath, theEvent);
        }

        public void Action(string eventName, Delegate theEvent)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafely(_agileLogger, eventPath, theEvent);
        }

        #endregion

        #region Action<T1>

        public void ActionAsyn<T1>(string eventName, Delegate theEvent, T1 t1)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafelyAsyn(_agileLogger, eventPath, theEvent, t1);
        }

        public void Action<T1>(string eventName, Delegate theEvent, T1 t1)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafely(_agileLogger, eventPath, theEvent, t1);
        }

        #endregion

        #region Action<T1, T2>

        public void ActionAsyn<T1, T2>(string eventName, Delegate theEvent, T1 t1, T2 t2)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafelyAsyn(_agileLogger, eventPath, theEvent, t1, t2);
        }

        public void Action<T1, T2>(string eventName, Delegate theEvent, T1 t1, T2 t2)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafely(_agileLogger, eventPath, theEvent, t1, t2);
        }

        #endregion

        #region Action<T1, T2, T3>

        public void ActionAsyn<T1, T2, T3>(string eventName, Delegate theEvent, T1 t1, T2 t2, T3 t3)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafelyAsyn(_agileLogger, eventPath, theEvent, t1, t2, t3);
        }

        public void Action<T1, T2, T3>(string eventName, Delegate theEvent, T1 t1, T2 t2, T3 t3)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafely(_agileLogger, eventPath, theEvent, t1, t2, t3);
        }

        #endregion

        #region Action<T1, T2, T3 ,T4>

        public void ActionAsyn<T1, T2, T3, T4>(string eventName, Delegate theEvent, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafelyAsyn(_agileLogger, eventPath, theEvent, t1, t2, t3, t4);
        }

        public void Action<T1, T2, T3, T4>(string eventName, Delegate theEvent, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var eventPath = string.Format("{0}.{1}", PublisherFullName, eventName);
            EventUtil.SpringEventSafely(_agileLogger, eventPath, theEvent, t1, t2, t3, t4);
        }

        #endregion
    }
}