using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus.Handlers
{
    /// <summary>
    /// Undirect base interface for all event handlers.
    /// Implement <see cref="IEventHandler{TEventData}"/> instead of this one.
    /// </summary>
    public interface IEventHandler
    {
    }
}
