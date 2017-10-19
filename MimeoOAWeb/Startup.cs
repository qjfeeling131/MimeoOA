using Abp.DoNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using MimeoOAWeb.Core.Filters;
using MimeoOAWeb.Core.Infrastructure;
using MimeoOAWeb.Core.Module;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;

namespace MimeoOAWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                           .RequireAuthenticatedUser()
                           .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "1.0 alpha",  //beta
                    Title = "MimeoCore API",
                    Description = "MimeoOA API",
                    TermsOfService = "None",
                    Contact = new Contact { Email = "marketplace@xtremeprog.com", Name = @"Pari, Sholto, Jesse", Url = @"www.gizwits.com" },
                    License = new License { Name = "MIT" },
                });
                //MimeoOAWeb
                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(basePath, "MimeoOAWeb.xml");
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                c.IncludeXmlComments(xmlPath);
            });
            //Set the amount of Master/Slave, and I know this setting is so digusting, but it need to do it for the moment. I will optimize this function as a middleware to implement the logic of Master/Slave
            services.Configure<Abp.EntityFrameworkCore.EFCoreDataBaseOptions>(options =>
            {
                MimeoOAConfiguration mimeoConfiguration = new MimeoOAConfiguration();
                Configuration.GetSection("EntityFrameworkCore:MimeoConfiguration").Bind(mimeoConfiguration);
                options.DbConnections = new Dictionary<Abp.DBSelector, string>();
                options.DbConnections.Add(Abp.DBSelector.Master, mimeoConfiguration.MasterConnectionString);
                options.DbConnections.Add(Abp.DBSelector.Slave, mimeoConfiguration.SalveConnectIonString);
            });

            //Set the Redis Cache Configuration
            services.Configure<Abp.RedisCache.AbpRedisCacheOptions>(options =>
            {
                Abp.RedisCache.AbpRedisCacheConfiguration redisCacheConfiguration = new Abp.RedisCache.AbpRedisCacheConfiguration();
                Configuration.GetSection("RedisCacheSection:Connections").Bind(redisCacheConfiguration);
                options.DbConnections = new Dictionary<Abp.DBSelector, string>();
                options.DbConnections.Add(Abp.DBSelector.Master, redisCacheConfiguration.MasterConnection);
                options.DbConnections.Add(Abp.DBSelector.Slave, redisCacheConfiguration.SlaveConnection);
                options.DatabaseId = redisCacheConfiguration.DataBaseId;
            });
            return services.AddAbp<MimeoOAModule>(Configuration);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            app.UserAbp(Configuration);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mimeo OA API 1.0.1");
            });
            app.UseMvc();
        }
    }
}
