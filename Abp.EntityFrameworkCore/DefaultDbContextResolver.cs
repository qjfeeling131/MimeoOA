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
        private readonly Dictionary<DBSelector, DbContext> _cacheDbContext = new Dictionary<DBSelector, DbContext>();
        private readonly Dictionary<int, DbContext> _slaveCacheDbContext = new Dictionary<int, DbContext>();
        private readonly IAbpDbContextConfigurer<TDbContext> _dbContextConfigurer;
        private readonly EFCoreDataBaseOptions _dbOptions;
        public DefaultDbContextResolver(IAbpDbContextConfigurer<TDbContext> dbContextConfigurer, IOptions<EFCoreDataBaseOptions> dbOptions)
        {
            this._dbContextConfigurer = dbContextConfigurer;
            this._dbOptions = dbOptions.Value;
        }
        public DbContext Resolve(DBSelector dbSelector = DBSelector.Master)
        {
            DbContext dbContext = null;
            if (dbSelector.Equals(DBSelector.Master))
            {
                _cacheDbContext.TryGetValue(dbSelector, out dbContext);
            }
            //ISSUE:A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe
            //We do it in temprary, we must optimzie the Framewrok to resolve the multiple thread to call DbContext.
            else
            {
                _slaveCacheDbContext.TryGetValue(Thread.CurrentThread.ManagedThreadId, out dbContext);
            }
            if (dbContext != null)
            {
                return dbContext;
            }
            var configurer = new AbpDbContextConfiguration<TDbContext>(_dbOptions.DbConnections[dbSelector]);
            _dbContextConfigurer.Configure(configurer);
            var actualContext = typeof(TDbContext);
            dbContext = (DbContext)Activator.CreateInstance(actualContext, configurer.DbContextOptions.Options);
            if (dbSelector.Equals(DBSelector.Master))
            {
                _cacheDbContext.Add(dbSelector, dbContext);
            }
            else
            {
                _slaveCacheDbContext.Add(Thread.CurrentThread.ManagedThreadId, dbContext);
            }
           
            return dbContext;
        }
        public void Dispose()
        {
            foreach (var item in _cacheDbContext)
            {
                if (item.Value.Database.CurrentTransaction != null)
                {
                    item.Value.Database.CurrentTransaction.Dispose();
                }
                item.Value.Dispose();
            }
            foreach (var item in _slaveCacheDbContext)
            {
                item.Value.Dispose();
            }
            _cacheDbContext.Clear();
            _slaveCacheDbContext.Clear();
        }
    }
}
