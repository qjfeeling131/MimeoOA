using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Dependency
{
   public interface IAutofacInstaller
    {

        IContainer Install(ContainerBuilder builder);
    }
}
