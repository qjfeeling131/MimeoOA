using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Domain.Services
{
    [UnitOfWork(isTransactional: true)]
    public abstract class DomainService : IDomainService
    {
    }
}
