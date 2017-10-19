using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Abp.Configuration.Startup;
using System.Collections.Immutable;

namespace Abp.Runtime.Caching.Configuration
{
    public class CachingConfiguration : ICachingConfiguration
    {
        //public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        private readonly List<ICacheConfigurator> _configurators;
        public IReadOnlyList<ICacheConfigurator> Configurators { get { return _configurators.ToImmutableList(); } }

        public void Configure(string cacheName, Action<ICache> initAction)
        {
            _configurators.Add(new CacheConfigurator(cacheName, initAction));
        }

        public void ConfigureAll(Action<ICache> initAction)
        {
            _configurators.Add(new CacheConfigurator(initAction));
        }
    }
}
