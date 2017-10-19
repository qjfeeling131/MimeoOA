using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Abp.Domain.Entities;
using Autofac;

namespace Abp.EntityFrameworkCore.Repositories
{
    public class EfCoreGenericRepositoryRegistrar
    {
        public void RegisterForDbContext(Type dbContextType, ContainerBuilder builder)
        {
            var autoRepositoryAttr = EfCoreAutoRepositoryTypes.Default;

            foreach (var entityTypeInfo in DbContextHelper.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(Guid))
                {
                    var genericRepositoryType = autoRepositoryAttr.RepositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    var implType = autoRepositoryAttr.RepositoryImplementation.GetGenericArguments().Length == 1
                            ? autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType);
                    //builder.RegisterGeneric(implType).As(genericRepositoryType).InstancePerLifetimeScope();
                    builder.RegisterType(implType).As(genericRepositoryType).InstancePerLifetimeScope();
               }

                //var genericRepositoryTypeWithPrimaryKey = autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
                //if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                //{
                //var implTypePrimarykey = autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                //            ? autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                //            : autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType, primaryKeyType);
            }
        }
    }
}
