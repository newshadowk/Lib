using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Base
{
    public class Dispatcher
    {
        private readonly BufferBlock<InvokeAction> _actionQ = new BufferBlock<InvokeAction>();

        public Dispatcher()
        {
            Init();
        }

        private void Init()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var action = await _actionQ.ReceiveAsync();
                    action.Action.Invoke();
                    action.InvokedFlag.Post(true);
                }
            });
        }

        public void Invoke(Action action)
        {
            var invokeAction = new InvokeAction(action);
            _actionQ.Post(invokeAction);
            invokeAction.InvokedFlag.Receive();
        }

        public async Task InvokeAsync(Action action)
        {
            var invokeAction = new InvokeAction(action);
            _actionQ.Post(invokeAction);
            await invokeAction.InvokedFlag.ReceiveAsync();
        }

        public Task BeginInvoke(Action action)
        {
            return InvokeAsync(action);
        }
    }
}
