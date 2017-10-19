using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Threading.Runtime.Caching
{
    public abstract class CacheBase : ICache
    {
        public string Name { get; }
        public TimeSpan DefaultSlidingExpireTime { get; set; }
        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly object SyncObj = new object();
        public CacheBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
        }
        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            this.Clear();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
        }

        public virtual object Get(string key, Func<string, object> factory)
        {
            var cacheKey = key;
            var item = GetOrDefault(key);
            if (item == null)
            {
                lock (SyncObj)
                {
                    item = GetOrDefault(key);
                    if (item == null)
                    {
                        item = factory(key);
                        if (item == null)
                        {
                            return null;
                        }

                        Set(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory)
        {
            var cacheKey = key;
            var item = await GetOrDefaultAsync(key);
            if (item == null)
            {
                item = await factory(key);
                if (item == null)
                {
                    return null;
                }
                await SetAsync(cacheKey, item);
            }

            return item;
        }

        public abstract object GetOrDefault(string key);

        public virtual Task<object> GetOrDefaultAsync(string key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public abstract void Remove(string key);

        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = default(TimeSpan?), TimeSpan? absoluteExpireTime = default(TimeSpan?));

        public virtual Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = default(TimeSpan?), TimeSpan? absoluteExpireTime = default(TimeSpan?))
        {
            Set(key, value, slidingExpireTime);
            return Task.FromResult(0);
        }
    }
}
