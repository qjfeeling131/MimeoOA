using Abp.Dependency;
using Abp.DoNetCore.Filters;
using Abp.Modules;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.DoNetCore.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Abp.DoNetCore.Handlers;

namespace Abp.DoNetCore
{
    public static class AbpServiceCollectionExtensions
    {
        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly static SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public static IServiceProvider AddAbp<TSartModule>(this IServiceCollection services, IConfigurationRoot configuration) where TSartModule : AbpModule
        {

            //Add the authoriaztion 

            //services.AddAuthorization(options => options.AddPolicy("MimeoOA", policy => policy.RequireClaim("MimeoUser", "test")));
            services.AddAuthorization(options => options.AddPolicy(MimeoOAPolicyType.PolicyName, policy => policy.Requirements.Add(new JwtUserAhtorizationRequirement())));
            services.AddSingleton<IAuthorizationHandler, AbpAuthorizationHandler>();
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.Subject = jwtAppSettingOptions[nameof(JwtIssuerOptions.Subject)];
                options.ValidFor = TimeSpan.FromMinutes(Convert.ToDouble(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)]));
                options.SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            //Add the DotNet Core MVC filters
            services.AddScoped<ExceptionFilter>();
            services.AddScoped<PermissionFilter>();
            var abpBootstrapper = AddAbpBootstrapper<TSartModule>(services, IocManager.Instance);
            abpBootstrapper.IocManager.Builder.Populate(services);
            abpBootstrapper.IocManager.BuildComponent();

            return new AutofacServiceProvider(abpBootstrapper.IocManager.IocContainer);
        }
        private static AbpBootstrapper AddAbpBootstrapper<TStartModule>(IServiceCollection services, IIocManager iocManager) where TStartModule : AbpModule
        {
            var abpBootstrapper = AbpBootstrapper.Create<TStartModule>(iocManager);
            services.AddSingleton(abpBootstrapper);
            abpBootstrapper.Initialize();
            return abpBootstrapper;
        }
    }
}
