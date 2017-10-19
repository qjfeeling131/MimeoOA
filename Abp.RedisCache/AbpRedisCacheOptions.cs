using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RedisCache
{
    public class AbpRedisCacheOptions
    {
        public Dictionary<DBSelector, string> DbConnections { get; set; }

        public int DatabaseId { get; set; }
    }
}
