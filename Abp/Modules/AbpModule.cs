using Abp.Collections.Extensions;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Abp.Domain.Uow;

namespace Abp.Modules
{
    public abstract class AbpModule : Autofac.Module
    {
        public virtual void PreInitialize(ContainerBuilder builder)
        {

        }

        public abstract void Initialize(ContainerBuilder builder);

        public virtual void PostInitialize(ContainerBuilder builder)
        {

        }
        public virtual void Shutdown()
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            this.PreInitialize(builder);
            this.Initialize(builder);
            this.PostInitialize(builder);
            base.Load(builder);
        }

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="impl">The type that implements <paramref name="type"/></param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        protected void Register(Type type, Type impl, ContainerBuilder builder, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    builder.RegisterType(impl).As(type).SingleInstance().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
                case DependencyLifeStyle.Transient:
                    builder.RegisterType(impl).As(type).InstancePerLifetimeScope().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
                default:
                    builder.RegisterType(impl).As(type).SingleInstance().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
            }
        }
        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <typeparam name="TType">Registering type</typeparam>
        /// <typeparam name="TImpl">The type that implements <see cref="TType"/></typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        protected void Register<TType, TImpl>(ContainerBuilder builder, DependencyLifeStyle lifeStyle)
             where TType : class
             where TImpl : class, TType
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    builder.RegisterType<TImpl>().As<TType>().SingleInstance().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
                case DependencyLifeStyle.Transient:
                    builder.RegisterType<TImpl>().As<TType>().InstancePerLifetimeScope().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
                default:
                    builder.RegisterType<TImpl>().As<TType>().SingleInstance().EnableInterfaceInterceptors().InterceptedBy(typeof(UnitOfWorkInterceptor));
                    break;
            }

        }
        public static bool IsAbpModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType && typeof(AbpModule).IsAssignableFrom(type);
        }

        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsAbpModule(moduleType))
            {
                throw new AbpInitializationException("This type is not an ABP module:" + moduleType.AssemblyQualifiedName);
            }
            var list = new List<Type>();
            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }
            return list;
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesResursively(list, moduleType);
            list.AddIfNotContains(typeof(AbpKernelModule));
            return list;
        }

        private static void AddModuleAndDependenciesResursively(List<Type> modules, Type module)
        {
            if (!IsAbpModule(module))
            {
                throw new AbpInitializationException("This type is not an ABP module: " + module.AssemblyQualifiedName);
            }

            if (modules.Contains(module))
            {
                return;
            }

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesResursively(modules, dependedModule);
            }
        }
    }
}
