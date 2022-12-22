namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class MovePriorityManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly int priorityMoveFactor;

        public MovePriorityManagementOperation(int priorityMoveFactor)
        {
            this.priorityMoveFactor = priorityMoveFactor;
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            IEnumerable<Rule<TContentType, TConditionType>> updatedPrioritiesRules = rules.Select(r =>
            {
                r.Priority += this.priorityMoveFactor;
                return r;
            }).ToList();

            return Task.FromResult(updatedPrioritiesRules);
        }
    }
}