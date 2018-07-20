using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Base
{
    public class PriorityBufferBlock<T>
    {
        private readonly PriorityBlock<T> _priorityBlock = new PriorityBlock<T>();
        private readonly BufferBlock<T> _highBlock;
        private readonly BufferBlock<T> _lowBlock;

        public PriorityBufferBlock()
        {
            _highBlock = new BufferBlock<T>();
            _lowBlock = new BufferBlock<T>();
            _highBlock.LinkTo(_priorityBlock.HighPriorityTarget);
            _lowBlock.LinkTo(_priorityBlock.LowPriorityTarget);
        }

        public void Post(T item, bool isHigh = false)
        {
            if (isHigh)
                _highBlock.Post(item);
            else
                _lowBlock.Post(item);
        }

        public int Count => _highBlock.Count + _lowBlock.Count;

        public async Task<T> ReceiveAsync()
        {
            return await _priorityBlock.Source.ReceiveAsync();
        }
    }
}