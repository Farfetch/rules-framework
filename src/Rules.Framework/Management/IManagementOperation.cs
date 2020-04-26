namespace Rules.Framework.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal interface IManagementOperation<TContentType, TConditionType>
    {
        Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules);
    }
}