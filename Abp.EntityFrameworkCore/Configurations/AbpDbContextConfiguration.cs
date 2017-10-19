using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore.Configurations
{
    public class AbpDbContextConfiguration<TDbContext>
         where TDbContext : DbContext
    {
        public string ConnectionString { get; internal set; }

        public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

        public AbpDbContextConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }
    }
}
