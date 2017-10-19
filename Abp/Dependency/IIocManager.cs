using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Dependency
{
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        IContainer IocContainer { get; }

        ContainerBuilder Builder { get; }

        ContainerBuilder PreBuilder { get; }
        new bool IsRegistered(Type type);

        /// <summary>
        /// the new is in order to hide the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        new bool IsRegistered<T>();

        bool BuildComponent();
    }
}
