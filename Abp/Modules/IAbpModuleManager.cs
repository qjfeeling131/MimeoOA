using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Modules
{
    public interface IAbpModuleManager
    {
        AbpModuleInfo StartupModule { get; }

        IReadOnlyList<AbpModuleInfo> Modules { get; }

        List<Type> ModuleAssemblies { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}
