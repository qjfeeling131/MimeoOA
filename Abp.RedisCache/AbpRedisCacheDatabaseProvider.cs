using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace Abp.RedisCache
{
    public class AbpRedisCacheDatabaseProvider : IAbpRedisCacheDatabaseProvider
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private readonly IOptions<AbpRedisCacheOptions> _options;
        public AbpRedisCacheDatabaseProvider(IOptions<AbpRedisCacheOptions> redisCacheOptions)
        {
            _options = redisCacheOptions;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        public void Dispose()
        {
            
        }

        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.Value.DatabaseId);
        }

        /// <summary>
        /// Create the Connections to Redis, but I Don't have better solution to resolve the Master-Slave Mode on it, It need to be discuss how to implemnt the Master-Slave mode
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            var configOption = new ConfigurationOptions();
            configOption.EndPoints.Add(_options.Value.DbConnections[DBSelector.Master]);
            configOption.AbortOnConnectFail = false;
            configOption.AllowAdmin = true;
            return ConnectionMultiplexer.Connect(configOption);
        }
    }

}
