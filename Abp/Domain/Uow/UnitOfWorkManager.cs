using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Transactions;

namespace Abp.Domain.Uow
{
    internal class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IUnitOfWork _iocUnitOfWork;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions _defaultOptions;
        public IActiveUnitOfWork Current
        {
            get
            {
                return _currentUnitOfWorkProvider.Current;
            }
        }

        public UnitOfWorkManager(IUnitOfWork iocUnitOfWork, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IUnitOfWorkDefaultOptions defaultOptions)
        {
            _iocUnitOfWork = iocUnitOfWork;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _defaultOptions = defaultOptions;
        }
        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(_defaultOptions);
            //if (options.Scope == TransactionScopeOption.Required && _currentUnitOfWorkProvider != null)
            //{
            //    return new InnerUnitOfWorkCompleteHandle();
            //}

            var uow = _iocUnitOfWork;
            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                uow.Dispose();
            };

            uow.Begin(options);

            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }


        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }
    }
}
