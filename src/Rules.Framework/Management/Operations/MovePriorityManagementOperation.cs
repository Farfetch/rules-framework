namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class MovePriorityManagementOperation : IManagementOperation
    {
        private readonly int priorityMoveFactor;

        public MovePriorityManagementOperation(int priorityMoveFactor)
        {
            this.priorityMoveFactor = priorityMoveFactor;
        }

        public Task<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            IEnumerable<Rule> updatedPrioritiesRules = rules.Select(r =>
            {
                r.Priority += this.priorityMoveFactor;
                return r;
            }).ToList();

            return Task.FromResult(updatedPrioritiesRules);
        }
    }
}