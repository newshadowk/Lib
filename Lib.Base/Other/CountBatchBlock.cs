using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Base
{
    public class CountBatchBlock<T>
    {
        private readonly BatchBlock<T> _batchBlock;
        private readonly SyncList<T> _syncList;
        private int _batchCount;

        public CountBatchBlock(int batchSize)
        {
            _batchBlock = new BatchBlock<T>(batchSize);
            _syncList = new SyncList<T>(batchSize);
            _batchCount = 0;
        }
        
        public int BatchCount => _batchCount;

        public int BatchSize => _batchBlock.BatchSize;

        public void Post(T item)
        {
            _batchBlock.Post(item);
            _syncList.Add(item);
            Interlocked.Increment(ref _batchCount);
        }

        public async Task<T[]> ReceiveAsync()
        {
            //todo Count need be remove when ReceiveAsync
            return await _batchBlock.ReceiveAsync();
        }
    }
}