
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore
{
    public class AbpDbContext : DbContext
    {
        public DBSelector DBSelector { get; set; }
        public AbpDbContext(DbContextOptions options)
          : base(options)
        {
        }
    }
}
