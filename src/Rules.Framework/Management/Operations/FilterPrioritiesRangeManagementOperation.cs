namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class FilterPrioritiesRangeManagementOperation : IManagementOperation
    {
        private readonly int? bottomPriorityThreshold;
        private readonly int? topPriorityThreshold;

        public FilterPrioritiesRangeManagementOperation(int? topPriorityThreshold, int? bottomPriorityThreshold)
        {
            this.topPriorityThreshold = topPriorityThreshold;
            this.bottomPriorityThreshold = bottomPriorityThreshold;
        }

        public Task<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            var filteredRules = rules;

            if (this.topPriorityThreshold.HasValue)
            {
                filteredRules = filteredRules.Where(r => r.Priority >= this.topPriorityThreshold);
            }

            if (this.bottomPriorityThreshold.HasValue)
            {
                filteredRules = filteredRules.Where(r => r.Priority <= this.bottomPriorityThreshold);
            }

            return Task.FromResult(filteredRules);
        }
    }
}