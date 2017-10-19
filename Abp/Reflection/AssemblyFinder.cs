using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Abp.Reflection
{
    public class AssemblyFinder : IAssemblyFinder
    {
        private readonly IAbpModuleManager abpModuleManager;
        public AssemblyFinder(IAbpModuleManager abpModuleManager)
        {
            this.abpModuleManager = abpModuleManager;
        }
        public List<Assembly> GetAllAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var module in abpModuleManager.ModuleAssemblies)
            {
                assemblies.Add(module.GetTypeInfo().Assembly);

            }
            return assemblies;
        }
    }
}
