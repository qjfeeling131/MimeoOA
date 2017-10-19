using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Modules
{
    public class AbpModuleInfo
    {
        public Assembly Assembly { get; }

        public Type Type { get; }

        public AbpModule Instance { get; }

        public List<AbpModuleInfo> Dependencies { get; }

        public AbpModuleInfo([NotNull] Type type, [NotNull] AbpModule instance)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            Type = type;
            Instance = instance;
            Dependencies = new List<AbpModuleInfo>();
        }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ??
                   Type.FullName;
        }
    }
}
