using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Events.Bus.Exceptions
{
    public class AbpHandledExceptionData: ExceptionData
    {
        public AbpHandledExceptionData(Exception exception) : base(exception) { }
    }
}
