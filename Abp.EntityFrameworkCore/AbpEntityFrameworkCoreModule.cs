using System;
using Abp.Modules;
using Autofac;
using Abp.EntityFrameworkCore.Uow;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Reflection;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;

namespace Abp.EntityFrameworkCore
{
    public class AbpEntityFrameworkCoreModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;
        public AbpEntityFrameworkCoreModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        public override void PreInitialize(ContainerBuilder builder)
        {
            builder.RegisterType<EfCoreUnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        }
        public override void Initialize(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfCoreRepositoryBaseOfEntity<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            //RegisterGenericRepositoriesAndMatchDbContexes(builder);
        }

        public override void PostInitialize(ContainerBuilder builder)
        {
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes(ContainerBuilder builder)
        {
            var dbContextTypes =
              _typeFinder.Find(type =>
                  type.GetTypeInfo().IsPublic &&
                  !type.GetTypeInfo().IsAbstract &&
                  type.GetTypeInfo().IsClass &&
                  typeof(AbpDbContext).IsAssignableFrom(type)
                  );

            if (dbContextTypes.IsNullOrEmpty())
            {
                return;
            }
            EfCoreGenericRepositoryRegistrar register = new EfCoreGenericRepositoryRegistrar();

            foreach (var dbContextType in dbContextTypes)
            {
                register.RegisterForDbContext(dbContextType, builder);
            }
        }
    }
}
