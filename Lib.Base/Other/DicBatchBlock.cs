using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib.Base
{
    public class DicBatchBlock<TKey, TValue> where TKey:struct
    {
        private readonly SyncDictionary<TKey, CountBatchBlock<TValue>> _dic = new SyncDictionary<TKey, CountBatchBlock<TValue>>();

        public void CreateBatch(TKey key, int count)
        {
            lock (_dic.SyncRoot)
            {
                if (_dic.TryGetValue(key, out var block))
                    throw new ArgumentException("Already have key:" + key);
                block = new CountBatchBlock<TValue>(count);
                _dic.Add(key, block);
            }
        }

        public void Post(TKey key, TValue value)
        {
            if (!_dic.TryGetValue(key, out var block))
                throw new ArgumentOutOfRangeException($"Key is not found:{key}");
            block.Post(value);
        }

        public (int batchCount, int batchSize) GetCount(TKey key)
        {
            if (!_dic.TryGetValue(key, out var block))
                throw new ArgumentOutOfRangeException($"Key is not found:{key}");
            return (block.BatchCount, block.BatchSize);
        }

        public async Task<TValue[]> ReceiveAsync(TKey key)
        {
            if (!_dic.TryGetValue(key, out var block))
                throw new KeyNotFoundException($"key:{key}");
            var ret = await block.ReceiveAsync();
            _dic.Remove(key);
            return ret;
        }
    }
}