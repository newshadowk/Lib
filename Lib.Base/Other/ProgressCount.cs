using System;
using System.Collections.Generic;

namespace Lib.Base
{
    public class ProgressCount
    {
        private readonly List<(Action<int> action, int rate)> _callBacks = new List<(Action<int> action, int rate)>();

        private readonly Action<int> _outProgressCallback;

        public ProgressCount(Action<int> outProgressCallback)
        {
            _outProgressCallback = outProgressCallback;
        }

        public Action<int> CreateProgressCallback(int rate)
        {
            int lastProgress = 0;
            _callBacks.ForEach(i => lastProgress += i.rate);

            void Action(int i)
            {
                float fRate = (float)rate / 100;
                var currP = (int)(i * fRate);
                var p = lastProgress + currP;
                _outProgressCallback.Invoke(p);
            }

            _callBacks.Add((Action, rate));

            return Action;
        }
    }
}