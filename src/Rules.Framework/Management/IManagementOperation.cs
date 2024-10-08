namespace Rules.Framework.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal interface IManagementOperation
    {
        Task<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules);
    }
}