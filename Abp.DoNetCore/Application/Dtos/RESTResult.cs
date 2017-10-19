using Abp.DoNetCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.DoNetCore.Application.Dtos
{
    public class RESTResult
    {
        public RESTStatus Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
