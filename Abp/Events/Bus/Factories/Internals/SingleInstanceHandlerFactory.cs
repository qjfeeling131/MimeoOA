using Abp.Events.Bus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus.Factories.Internals
{
    internal class SingleInstanceHandlerFactory:IEventHandlerFactory
    {
        public IEventHandler HandlerInstance { get; private set; }

        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        public void ReleaseHandler(IEventHandler handler)
        {
        }
    }
}
