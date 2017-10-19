using Abp.Dependency;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Autofac.Core;

namespace Abp
{
    public class AbpKernelModule : AbpModule
    {
        public override void PreInitialize(ContainerBuilder builder)
        {
        }

        public override void PostInitialize(ContainerBuilder builder)
        {
            builder.RegisterType<EventBus>().As<IEventBus>().InstancePerLifetimeScope(); ;

        }
        public override void Initialize(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkOptions>().As<IUnitOfWorkDefaultOptions>().InstancePerLifetimeScope(); ;
            builder.RegisterType<CallContextCurrentUnitofWorkProvider>().As<ICurrentUnitOfWorkProvider>().InstancePerLifetimeScope(); ;
            builder.RegisterType<UnitOfWorkManager>().As<IUnitOfWorkManager>().Named<IUnitOfWorkManager>("IUnitOfWorkManager").InstancePerLifetimeScope(); ;
            builder.RegisterType<UnitOfWorkInterceptor>().WithParameter(new ResolvedParameter((pi, cts) => pi.Name == "unitOfWorkManager", (pi, ctx) => ctx.ResolveNamed<IUnitOfWorkManager>("IUnitOfWorkManager"))).InstancePerLifetimeScope(); ;
        }
    }
}   
