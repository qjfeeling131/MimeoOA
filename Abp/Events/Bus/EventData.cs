//using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus
{
    /// <summary>
    /// Implements <see cref="IEventData"/> and provides a base for event data classes.
    /// </summary>
    public class EventData : IEventData
    {
        public object EventSource { get; set; }

        public DateTime EventTime { get; set; }

        public EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}
