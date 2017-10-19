using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus.Exceptions
{
    public class ExceptionData : EventData
    {
        public Exception Exception { get; private set; }

        public ExceptionData(Exception exception)
        {
            Exception = exception;
        }
    }
}
