using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RedisCache
{
    public class AbpRedisCacheConfiguration
    {
        public string MasterConnection { get; set; }
        public string SlaveConnection { get; set; }
        public int DataBaseId { get; set; }
    }
}
