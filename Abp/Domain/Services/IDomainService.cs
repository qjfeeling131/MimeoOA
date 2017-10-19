using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Domain.Services
{
    public interface IDomainService: ITransientDependency
    {
    }
}
