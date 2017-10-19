using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore.Repositories
{
    public static class EfCoreAutoRepositoryTypes
    {
        public static AutoRepositoryTypesAttribute Default { get; private set; }

        static EfCoreAutoRepositoryTypes()
        {
            Default = new AutoRepositoryTypesAttribute(
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(EfCoreRepositoryBase<>)
                //typeof(EfCoreRepositoryBase<,,>)
            );
        }
    }
}
