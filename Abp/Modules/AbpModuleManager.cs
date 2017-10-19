using Abp.Dependency;
using Abp.Reflection;
using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Modules
{
    public class AbpModuleManager : IAbpModuleManager
    {
        public AbpModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<AbpModuleInfo> Modules => _modules.ToImmutableList();

        public List<Type> ModuleAssemblies { get => _ModuleAssemblies; set => _ModuleAssemblies = value; }
        private AbpModuleCollection _modules;

        private List<Type> _ModuleAssemblies;

        private readonly IIocManager _iocManager;
        public AbpModuleManager(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public virtual void Initialize(Type startupModule)
        {
            _modules = new AbpModuleCollection(startupModule);
            _ModuleAssemblies = new List<Type>();
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module =>
            {
                _iocManager.RegisterModule(module.Instance);
            });
        }

        public virtual void ShutdownModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }

        private void LoadAllModules()
        {
            var moduleTypes = FindAllModuleTypes().Distinct().ToList();
            RegisterModules(moduleTypes);
            CreateModules(moduleTypes);
            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();
            SetDependencies();
        }

        private List<Type> FindAllModuleTypes()
        {
            var modules = AbpModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);
            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes)
        {
            var preContrainer = _iocManager.PreBuilder.Build();
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = preContrainer.Resolve(moduleType) as AbpModule;
                if (moduleObject == null)
                {
                    throw new AbpInitializationException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
                }
                var moduleInfo = new AbpModuleInfo(moduleType, moduleObject);

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            
            foreach (var moduleType in moduleTypes)
            {
                _iocManager.PreBuilder.RegisterType(moduleType);
                _ModuleAssemblies.Add(moduleType);
            }
            RegisterPreComponent();
        }

        private void RegisterPreComponent()
        {
            _iocManager.PreBuilder.RegisterType<AbpModuleManager>().As<IAbpModuleManager>().WithParameter(new NamedParameter("iocManager", this._iocManager)).WithProperty("ModuleAssemblies", this.ModuleAssemblies).SingleInstance();
            _iocManager.PreBuilder.RegisterType<AssemblyFinder>().As<IAssemblyFinder>().SingleInstance();
            _iocManager.PreBuilder.RegisterType<TypeFinder>().As<ITypeFinder>().SingleInstance();

        }
        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();
                foreach (var dependedModuleType in AbpModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new AbpInitializationException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
