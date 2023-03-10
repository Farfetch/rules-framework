namespace Rules.Framework.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Management.Operations;
    using Rules.Framework.Source;

    internal sealed class ManagementOperationsController<TContentType, TConditionType>
    {
        private readonly List<IManagementOperation<TContentType, TConditionType>> managementOperations;
        private readonly IEnumerable<Rule<TContentType, TConditionType>> rules;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;

        public ManagementOperationsController(IRulesSource<TContentType, TConditionType> rulesSource, IEnumerable<Rule<TContentType, TConditionType>> rules)
        {
            this.managementOperations = new List<IManagementOperation<TContentType, TConditionType>>();
            this.rulesSource = rulesSource;
            this.rules = rules;
        }

        public ManagementOperationsController<TContentType, TConditionType> AddRule(Rule<TContentType, TConditionType> rule)
            => this.AddOperation(new AddRuleManagementOperation<TContentType, TConditionType>(this.rulesSource, rule));

        public async Task ExecuteOperationsAsync()
        {
            IEnumerable<Rule<TContentType, TConditionType>> rulesIntermediateResult = rules;

            foreach (IManagementOperation<TContentType, TConditionType> managementOperation in this.managementOperations)
            {
                rulesIntermediateResult = await managementOperation.ApplyAsync(rulesIntermediateResult).ConfigureAwait(false);
            }
        }

        public ManagementOperationsController<TContentType, TConditionType> FilterFromThresholdPriorityToBottom(int thresholdPriority)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation<TContentType, TConditionType>(thresholdPriority, null));

        public ManagementOperationsController<TContentType, TConditionType> FilterPrioritiesRange(int topPriorityThreshold, int bottomPriorityThreshold)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation<TContentType, TConditionType>(topPriorityThreshold, bottomPriorityThreshold));

        public ManagementOperationsController<TContentType, TConditionType> IncreasePriority()
            => this.AddOperation(new MovePriorityManagementOperation<TContentType, TConditionType>(1));

        public ManagementOperationsController<TContentType, TConditionType> DecreasePriority()
            => this.AddOperation(new MovePriorityManagementOperation<TContentType, TConditionType>(-1));

        public ManagementOperationsController<TContentType, TConditionType> SetRuleForUpdate(Rule<TContentType, TConditionType> updatedRule)
            => this.AddOperation(new SetRuleForUpdateManagementOperation<TContentType, TConditionType>(updatedRule));

        public ManagementOperationsController<TContentType, TConditionType> UpdateRules()
            => this.AddOperation(new UpdateRulesManagementOperation<TContentType, TConditionType>(this.rulesSource));

        private ManagementOperationsController<TContentType, TConditionType> AddOperation(IManagementOperation<TContentType, TConditionType> managementOperation)
        {
            this.managementOperations.Add(managementOperation);

            return this;
        }
    }
}