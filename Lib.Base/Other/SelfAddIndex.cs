namespace Lib.Base
{
    public class SelfIncreaseIndex
    {
        private int _i;

        private readonly object _objLock = new object();

        public int CreateIndex()
        {
            lock (_objLock)
            {
                var ret = _i;
                _i++;
                return ret;
            }
        }
    }
}