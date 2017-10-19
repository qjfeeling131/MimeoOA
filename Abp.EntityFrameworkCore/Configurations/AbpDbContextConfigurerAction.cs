using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore.Configurations
{
    public class AbpDbContextConfigurerAction<TDbContext> : IAbpDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        public Action<AbpDbContextConfiguration<TDbContext>> Action { get; set; }

        public AbpDbContextConfigurerAction(Action<AbpDbContextConfiguration<TDbContext>> action)
        {
            Action = action;
        }

        public void Configure(AbpDbContextConfiguration<TDbContext> configuration)
        {
            Action(configuration);
        }
    }
}
