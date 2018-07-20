using System.Threading;

namespace Lib.Base
{
    public class ProgCount
    {
        private int _count;
        private readonly int _itemsCountPerTime;
        private readonly int _maxCount;

        public ProgCount(int maxCount, int itemsCountPerTime = 100)
        {
            _itemsCountPerTime = itemsCountPerTime;
            _maxCount = maxCount;
        }

        public bool Increment(out int progressValue)
        {
            var newCount = Interlocked.Increment(ref _count);
            progressValue = newCount * 100 / _maxCount;
            return newCount % _itemsCountPerTime == 0;
        }
    }
}