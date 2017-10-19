using System;
using System.Collections.Generic;
using System.Text;

namespace Abp
{
    public interface IDataBaseOptions
    {
        Dictionary<DBSelector,string> DbConnections { get; }
    }
}
