using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Common
{
    public enum RESTStatus
    {
        Success=101,
        Failed=102,
        NotData=103,
        DataExisted=104,
        PartialSuccess =105,
    }
}
