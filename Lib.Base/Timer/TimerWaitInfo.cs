using System;
using System.Diagnostics;
using System.Timers;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerWaitInfo
    {

        #region Fields

        private double _currentInterval;

        private readonly double _waitTime;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TimerWaitInfo(double waitTime)
        {
            _waitTime = waitTime;
            IsCompleted = false;
            Sign = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods Private

        /// <summary>
        /// ，（）
        /// </summary>
        private void CheckTimeOut()
        {
            var timer = new Timer(_waitTime);
            timer.Elapsed += (s, e) =>
                                 {
                                     Debug.WriteLine("CheckTimeOut");
                                     timer.Stop();
                                     OnCheckTimeComplete(new EventArgs());
                                 };
            timer.Start();
        }

        private void OnCheckTimeComplete(EventArgs e)
        {
            var handler = CheckTimeComplete;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Methods Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns>True</returns>
        public bool Calc(double interval)
        {
            _currentInterval += interval;
            if (_currentInterval >= _waitTime)
            {
                IsCompleted = true;
                CheckTimeOut();
                return true;
            }
            return false;
        }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CheckTimeComplete;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Sign { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleted { private set; get; }

        #endregion

    }
}
