using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
namespace Abp.Dependency
{
    public class BasicConventionalRegistrar:IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //Register type inherit by ITransienDependency
            context.IocManager.Builder.RegisterAssemblyTypes(context.Assembly).As<ITransientDependency>().InstancePerLifetimeScope();

            //Register Type inherit by ISingletonDependency
            context.IocManager.Builder.RegisterAssemblyTypes(context.Assembly).As<ISingletonDependency>().SingleInstance();
        }
    }
}
