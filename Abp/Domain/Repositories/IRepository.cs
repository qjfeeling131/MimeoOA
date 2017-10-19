using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Domain.Repositories
{
    public interface IRepository : ITransientDependency
    {
    }
}
