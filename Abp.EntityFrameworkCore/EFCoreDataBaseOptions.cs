using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore
{
    public class EFCoreDataBaseOptions
    {
        public Dictionary<DBSelector, string> DbConnections { get; set; }
    }
}
