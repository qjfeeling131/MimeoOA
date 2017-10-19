using Abp.Modules;
using System;
using Autofac;
using AutoMapper;
using Abp.Reflection;
using System.Reflection;

namespace Abp.AutoMapper
{
    public class AbpAutoMapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;
        private static bool _createdMappingsBefore;
        private static readonly object SyncObj = new object();

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        public override void Initialize(ContainerBuilder builder)
        {
            CreateMappings();
            builder.RegisterType<Mapper>().As<IMapper>();
        }

        public void CreateMappings()
        {
            lock (SyncObj)
            {
                //We should prevent duplicate mapping in an application, since Mapper is static.
                if (_createdMappingsBefore)
                {
                    return;
                }

                Mapper.Initialize(configuration =>
                {
                    FindAndAutoMapTypes(configuration);
                });

                _createdMappingsBefore = true;
            }
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = _typeFinder.Find(type =>
                    type.GetTypeInfo().IsDefined(typeof(AutoMapAttribute)) ||
                    type.GetTypeInfo().IsDefined(typeof(AutoMapFromAttribute)) ||
                    type.GetTypeInfo().IsDefined(typeof(AutoMapToAttribute))
            );
            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
        }
    }
}
