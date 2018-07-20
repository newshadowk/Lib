using System;
using System.Threading.Tasks.Dataflow;

namespace Lib.Base
{
    internal class InvokeAction
    {
        public Action Action { get; }

        public WriteOnceBlock<bool> InvokedFlag { get; } = new WriteOnceBlock<bool>(null);

        public InvokeAction(Action action)
        {
            Action = action;
        }
    }
}