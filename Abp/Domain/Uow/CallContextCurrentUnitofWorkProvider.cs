using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Domain.Uow
{
   public class CallContextCurrentUnitofWorkProvider: ICurrentUnitOfWorkProvider
    {
        public IUnitOfWork Current { get; set; }
    }
}
