using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore.Configurations
{
    public interface IAbpDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        void Configure(AbpDbContextConfiguration<TDbContext> configuration);
    }
}
