using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Abp.Runtime.Caching;
using Autofac.Core;

namespace Abp.RedisCache
{
    public class AbpRedisCacheModule : AbpModule
    {
        public override void Initialize(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultRedisCacheSerializer>().As<IRedisCacheSerializer>().Named<IRedisCacheSerializer>("redisCacheSerializer").InstancePerLifetimeScope();
            builder.RegisterType<AbpRedisCacheDatabaseProvider>().As<IAbpRedisCacheDatabaseProvider>().Named<IAbpRedisCacheDatabaseProvider>("redisCacheDabaseProvider").InstancePerLifetimeScope();
            builder.RegisterType<AbpRedisCache>().As<ICache>().WithParameters(new Parameter[] {

                new NamedParameter("name","RedisCache"),
                new ResolvedParameter((pi,ctx)=>pi.Name=="redisCacheDatabaseProvider",(pi,ctx)=>ctx.ResolveNamed<IAbpRedisCacheDatabaseProvider>("redisCacheDabaseProvider")),
                 new ResolvedParameter((pi,ctx)=>pi.Name=="serialiazer",(pi,ctx)=>ctx.ResolveNamed<IRedisCacheSerializer>("redisCacheSerializer")),
            }).InstancePerLifetimeScope();
        }
    }
}
