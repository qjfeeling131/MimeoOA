using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus
{
    /// <summary>
    /// Defines Interface for all Event Data classes.
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        /// The time when the event occured.
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// The object which triggers the event(optional).
        /// </summary>
        object EventSource { get; set; }
    }
}
