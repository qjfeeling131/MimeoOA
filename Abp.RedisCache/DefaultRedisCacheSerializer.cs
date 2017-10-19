using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Abp.Json;

namespace Abp.RedisCache
{
    public class DefaultRedisCacheSerializer : IRedisCacheSerializer
    {
        public object Deserialize(RedisValue objbyte)
        {
            return JsonSerializationHelper.DeserializeWithType(objbyte);
        }

        public string Serialize(object value, Type type)
        {
            return JsonSerializationHelper.SerializeWithType(value, type);
        }
    }
}
