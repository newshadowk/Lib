using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Base
{
    public class PriorityBlock<T>
    {
        private readonly BufferBlock<T> _highPriorityTarget;

        public ITargetBlock<T> HighPriorityTarget => _highPriorityTarget;

        private readonly BufferBlock<T> _lowPriorityTarget;

        public ITargetBlock<T> LowPriorityTarget => _lowPriorityTarget;

        private readonly BufferBlock<T> _source;

        public ISourceBlock<T> Source => _source;

        public PriorityBlock()
        {
            var options = new DataflowBlockOptions { BoundedCapacity = 1 };

            _highPriorityTarget = new BufferBlock<T>(options);
            _lowPriorityTarget = new BufferBlock<T>(options);

            _source = new BufferBlock<T>(options);

            Task.Run(ForwardMessages);
        }

        private async Task ForwardMessages()
        {
            while (true)
            {
                await Task.WhenAny(
                    _highPriorityTarget.OutputAvailableAsync(),
                    _lowPriorityTarget.OutputAvailableAsync());

                if (_highPriorityTarget.TryReceive(out var item))
                {
                    await _source.SendAsync(item);
                }
                else if (_lowPriorityTarget.TryReceive(out item))
                {
                    await _source.SendAsync(item);
                }
                else
                {
                    // both input blocks must be completed
                    _source.Complete();
                    return;
                }
            }
        }
    }
}