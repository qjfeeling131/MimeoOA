using Abp.DoNetCore.Domain;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeoOAWeb.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimeoOAWeb.Core.MimeoDBContext
{
    public class MimeoOAContext : AbpDbContext
    {
        public DbSet<User> mo_user { get; set; }
        public DbSet<Role> mo_role { get; set; }
        public DbSet<UserInfo> mo_user_info { get; set; }
        public DbSet<UserRole> mo_user_role { get; set; }
        public DbSet<Permission> mo_permission { get; set; }
        public DbSet<RolePermission> mo_role_permission { get; set; }
        public DbSet<Department> mo_department { get; set; }
        public DbSet<UserDepartment> mo_user_department { get; set; }
        public MimeoOAContext(DbContextOptions<MimeoOAContext> options):base(options)
        {
        }
    }
}
