using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.EntityFrameworkCore
{
    public interface IDbContextResolver:IDisposable
    {
        DbContext Resolve(DBSelector dbSelector);
    }
}
