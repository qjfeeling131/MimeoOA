using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.EntityFrameworkCore.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Linq;

namespace Abp.EntityFrameworkCore
{
    public class DefaultDbContextResolver<TDbContext> : IDbContextResolver where TDbContext : DbContext
    {
        //private readonly ThreadLocal<Dictionary<DBSelector, DbContext>> CacheDbContext = new ThreadLocal<Dictionary<DBSelector, DbContext>>(() => new Dictionary<DBSelector, DbContext>());
        private readonly Dictionary<DBSelector, DbContext> CacheDbContext = new Dictionary<DBSelector, DbContext>();
        private readonly Dictionary<int, DbContext> SlaveCacheDbContext = new Dictionary<int, DbContext>();
        private readonly IAbpDbContextConfigurer<TDbContext> dbContextConfigurer;
        private readonly EFCoreDataBaseOptions dbOptions;
        public DefaultDbContextResolver(IAbpDbContextConfigurer<TDbContext> dbContextConfigurer, IOptions<EFCoreDataBaseOptions> dbOptions)
        {
            this.dbContextConfigurer = dbContextConfigurer;
            this.dbOptions = dbOptions.Value;
        }
        public DbContext Resolve(DBSelector dbSelector = DBSelector.Master)
        {
            DbContext dbContext = null;
            if (dbSelector.Equals(DBSelector.Master))
            {
                CacheDbContext.TryGetValue(dbSelector, out dbContext);
            }
            //ISSUE:A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe
            //We do it in temprary, we must optimzie the Framewrok to resolve the multiple thread to call DbContext.
            else
            {
                SlaveCacheDbContext.TryGetValue(Thread.CurrentThread.ManagedThreadId, out dbContext);
            }
            if (dbContext != null)
            {
                return dbContext;
            }
            var configurer = new AbpDbContextConfiguration<TDbContext>(dbOptions.DbConnections[dbSelector]);
            dbContextConfigurer.Configure(configurer);
            var actualContext = typeof(TDbContext);
            dbContext = (DbContext)Activator.CreateInstance(actualContext, configurer.DbContextOptions.Options);
            if (dbSelector.Equals(DBSelector.Master))
            {
                CacheDbContext.Add(dbSelector, dbContext);
            }
            else
            {
                SlaveCacheDbContext.Add(Thread.CurrentThread.ManagedThreadId, dbContext);
            }
           
            return dbContext;
        }
        public void Dispose()
        {
            foreach (var item in CacheDbContext)
            {
                if (item.Value.Database.CurrentTransaction != null)
                {
                    item.Value.Database.CurrentTransaction.Dispose();
                }
                item.Value.Dispose();
            }
            foreach (var item in SlaveCacheDbContext)
            {
                item.Value.Dispose();
            }
            CacheDbContext.Clear();
            SlaveCacheDbContext.Clear();
        }
    }
}
