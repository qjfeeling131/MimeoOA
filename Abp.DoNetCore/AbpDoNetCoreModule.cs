using Abp.AutoMapper;
using Abp.DoNetCore.Application;
using Abp.DoNetCore.Domain.XmlObjects;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.RedisCache;
using Autofac;
using ExtendedXmlSerialization;
using ExtendedXmlSerialization.Autofac;

namespace Abp.DoNetCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpAutoMapperModule),typeof(AbpRedisCacheModule))]
    public class AbpDoNetCoreModule : AbpModule
    {
        public override void Initialize(ContainerBuilder builder)
        {
            Register<IUserAppService, UserAppService>(builder, Dependency.DependencyLifeStyle.Transient);
            Register<IDigitalAssetService, DigitalAssetService>(builder, Dependency.DependencyLifeStyle.Transient);
            Register<IAbpAuthorizationService, AbpAuthorizationService>(builder, Dependency.DependencyLifeStyle.Transient);
        }

        public override void PostInitialize(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
            builder.RegisterType<PageMenuConfig>().As<ExtendedXmlSerializerConfig<PageMenu>>().SingleInstance();
        }
    }
}
