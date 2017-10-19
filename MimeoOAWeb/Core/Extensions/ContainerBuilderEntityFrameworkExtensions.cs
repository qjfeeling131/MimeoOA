using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimeoOAWeb.Core.Extensions
{
    public static class ContainerBuilderEntityFrameworkExtensions
    {
        public static ContainerBuilder AddDbContext<TContext>(this ContainerBuilder builder,
     Action<DbContextOptionsBuilder, IConfiguration> optionsAction = null) where TContext : DbContext
        {
            if (optionsAction != null)
            {
                builder.Register<DbContextOptions<TContext>>(p => DbContextOptionsFactory<TContext>(optionsBuilder =>
                {
                    IConfiguration config = p.Resolve<IConfiguration>();
                    optionsAction(optionsBuilder, config);
                }));
            }
            else
            {
                builder.Register<DbContextOptions<TContext>>(_ => DbContextOptionsFactory<TContext>(null));
            }

            builder.Register<DbContextOptions>(p => p.Resolve<DbContextOptions<TContext>>()).InstancePerLifetimeScope();
            builder.RegisterType<TContext>().As<DbContext>().InstancePerLifetimeScope();
            return builder;
        }

        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            Action<DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            var options = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
            if (optionsAction != null)
            {
                var builder = new DbContextOptionsBuilder<TContext>(options);
                optionsAction(builder);
                options = builder.Options;
            }

            return options;
        }
    }
}
