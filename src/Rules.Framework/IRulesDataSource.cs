using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rules.Framework.Core;

namespace Rules.Framework
{
    public interface IRulesDataSource<TContentType, TConditionType>
    {
        Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd);
    }
}