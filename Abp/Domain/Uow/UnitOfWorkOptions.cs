using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Unit of work options.
    /// </summary>
    public class UnitOfWorkOptions : IUnitOfWorkDefaultOptions
    {
        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Can be used to enable/disable some filters. 
        /// </summary>
        public List<DataFilterConfiguration> FilterOverrides { get; private set; }

        TransactionScopeOption IUnitOfWorkDefaultOptions.Scope { get; set; }

        bool IUnitOfWorkDefaultOptions.IsTransactional { get; set; }

        /// <summary>
        /// It will add data filter when the action was executed before.
        /// </summary>
        public IReadOnlyList<DataFilterConfiguration> Filters { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkOptions"/> object.
        /// </summary>
        public UnitOfWorkOptions()
        {
            FilterOverrides = new List<DataFilterConfiguration>();
        }

        internal void FillDefaultsForNonProvidedOptions(IUnitOfWorkDefaultOptions defaultOptions)
        {
            //TODO: Do not change options object..?
            if (!IsTransactional.HasValue)
            {
                IsTransactional = defaultOptions.IsTransactional;
            }

            if (!Scope.HasValue)
            {
                Scope = defaultOptions.Scope;
            }

            if (!Timeout.HasValue && defaultOptions.Timeout.HasValue)
            {
                Timeout = defaultOptions.Timeout.Value;
            }
        }

        public void RegisterFilter(string filterName, bool isEnabledByDefault)
        {
            //throw new NotImplementedException();
        }

        public void OverrideFilter(string filterName, bool isEnabledByDefault)
        {
            //throw new NotImplementedException();
        }
    }
}
