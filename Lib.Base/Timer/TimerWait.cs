using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimerWait
    {

        #region Fields

        /// <summary>
        /// KeyGUID，Value
        /// </summary>
        private static readonly ConcurrentDictionary<string, TimerWaitInfo> CacheWaitParams = new ConcurrentDictionary<string, TimerWaitInfo>();

        /// <summary>
        /// 
        /// </summary>
        private static readonly ConcurrentDictionary<string, TimerWaitInfo> WaitParamsCompleted = new ConcurrentDictionary<string, TimerWaitInfo>();

        /// <summary>
        /// ，100。
        /// </summary>
        private const int MinInterval = 100;

        /// <summary>
        /// 
        /// </summary>
        private static readonly Timer TaskTimer = new Timer(MinInterval);

        /// <summary>
        /// 
        /// </summary>
        private static readonly object LockTimer = new object();

        #endregion

        #region Constructors

        static TimerWait()
        {
            TaskTimer.Elapsed += TaskTimerElapsed;
        }

        #endregion

        #region Methods Private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerWaitInfo"></param>
        private static void RemoveWaitParamsCompleted(TimerWaitInfo timerWaitInfo)
        {
            if (WaitParamsCompleted.ContainsKey(timerWaitInfo.Sign))
            {
                WaitParamsCompleted.TryRemove(timerWaitInfo.Sign, out timerWaitInfo);
                timerWaitInfo.CheckTimeComplete -= TimerWaitInfoCheckTimeComplete;
            }
        }

        #endregion

        #region Methods Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="guid"></param>
        public static void AddNewWait(double waitTime, out string guid)
        {
            var timerWaitInfo = new TimerWaitInfo(waitTime);
            timerWaitInfo.CheckTimeComplete += TimerWaitInfoCheckTimeComplete;
            guid = timerWaitInfo.Sign;
            CacheWaitParams.TryAdd(guid, timerWaitInfo);
            lock (LockTimer)
            {
                if (!TaskTimer.Enabled)
                    TaskTimer.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static bool GetWaitStatusWithRemove(string sign)
        {
            var result = false;
            if (WaitParamsCompleted.ContainsKey(sign))
            {
                result = WaitParamsCompleted[sign].IsCompleted;
                RemoveWaitParamsCompleted(WaitParamsCompleted[sign]);
            }
            return result;
        }

        private static void TimerWaitInfoCheckTimeComplete(object sender, EventArgs e)
        {
            var timerWaitInfo = (TimerWaitInfo)sender;
            RemoveWaitParamsCompleted(timerWaitInfo);
        }

        #endregion

        #region Events

        private static void TaskTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock(LockTimer)
            {
                if (CacheWaitParams.Count == 0)
                {
                    TaskTimer.Stop();
                    return;
                }
            }
            var removeKey = new List<string>();
            foreach (var cacheWaitParam in CacheWaitParams)
            {
                if (cacheWaitParam.Value.Calc(TaskTimer.Interval))
                {
                    removeKey.Add(cacheWaitParam.Key);
                }
            }
            foreach (var sign in removeKey)
            {
                TimerWaitInfo timerWaitInfo;
                CacheWaitParams.TryRemove(sign, out timerWaitInfo);
                if (timerWaitInfo != null && timerWaitInfo.IsCompleted)
                    WaitParamsCompleted.TryAdd(timerWaitInfo.Sign, timerWaitInfo);
            }
        }

        #endregion

    }
}
