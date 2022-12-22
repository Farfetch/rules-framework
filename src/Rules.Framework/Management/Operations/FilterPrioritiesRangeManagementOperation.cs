namespace Rules.Framework.Management.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class FilterPrioritiesRangeManagementOperation<TContentType, TConditionType> : IManagementOperation<TContentType, TConditionType>
    {
        private readonly int? topPriorityThreshold;
        private readonly int? bottomPriorityThreshold;

        public FilterPrioritiesRangeManagementOperation(int? topPriorityThreshold, int? bottomPriorityThreshold)
        {
            this.topPriorityThreshold = topPriorityThreshold;
            this.bottomPriorityThreshold = bottomPriorityThreshold;
        }

        public Task<IEnumerable<Rule<TContentType, TConditionType>>> ApplyAsync(IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            IEnumerable<Rule<TContentType, TConditionType>> filteredRules = rules;

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