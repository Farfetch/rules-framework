namespace Rules.Framework.Source
{
    using Rules.Framework.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal delegate Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesFilteredDelegate<TContentType, TConditionType>(
        GetRulesFilteredArgs<TContentType> args);
}
