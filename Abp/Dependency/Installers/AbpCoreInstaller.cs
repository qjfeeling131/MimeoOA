using Abp.Modules;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Dependency.Installers
{
    internal class AbpCoreInstaller : IAutofacInstaller
    {
        public IContainer Install(ContainerBuilder builder)
        {
            builder.RegisterType<IAbpModuleManager>().As<AbpModuleManager>();

            return builder.Build();
        }
    }
}
