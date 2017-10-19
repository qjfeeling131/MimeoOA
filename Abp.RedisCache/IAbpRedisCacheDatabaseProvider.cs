using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RedisCache
{
    public interface IAbpRedisCacheDatabaseProvider:IDisposable
    {
        IDatabase GetDatabase();
    }
}
