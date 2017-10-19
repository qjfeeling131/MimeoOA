using Abp.Domain.Entities;
using Abp.Threading.Runtime.Caching;
using StackExchange.Redis;
using System;

namespace Abp.RedisCache
{
    public class AbpRedisCache : CacheBase
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;

        public AbpRedisCache(string name, IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider, IRedisCacheSerializer serialiazer) : base(name)
        {
            _database = redisCacheDatabaseProvider.GetDatabase();
            _serializer = serialiazer;
        }

        public override void Clear()
        {
            //_database.keyd
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = _database.StringGet(GetLocalizedKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override void Remove(string key)
        {
            _database.KeyDelete(GetLocalizedKey(key));
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = default(TimeSpan?), TimeSpan? absoluteExpireTime = default(TimeSpan?))
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the Cache!");
            }

            var type = value.GetType();
            if (EntityHelper.IsEntity(type))
            {
                _database.StringSet(GetLocalizedKey(key), Serialize(value, type), absoluteExpireTime ?? slidingExpireTime ?? DefaultAbsoluteExpireTime ?? DefaultSlidingExpireTime);
            }
        }
        protected virtual string Serialize(object value, Type type)
        {
            return _serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue objbyte)
        {
            return _serializer.Deserialize(objbyte);
        }
        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }
    }
}
