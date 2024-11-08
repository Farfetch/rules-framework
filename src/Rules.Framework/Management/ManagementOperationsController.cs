namespace Rules.Framework.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Management.Operations;
    using Rules.Framework.Source;

    internal sealed class ManagementOperationsController
    {
        private readonly List<IManagementOperation> managementOperations;
        private readonly IEnumerable<Rule> rules;
        private readonly IRulesSource rulesSource;

        public ManagementOperationsController(IRulesSource rulesSource, IEnumerable<Rule> rules)
        {
            this.managementOperations = new List<IManagementOperation>();
            this.rulesSource = rulesSource;
            this.rules = rules;
        }

        public ManagementOperationsController AddRule(Rule rule)
            => this.AddOperation(new AddRuleManagementOperation(this.rulesSource, rule));

        public ManagementOperationsController DecreasePriority()
            => this.AddOperation(new MovePriorityManagementOperation(-1));

        public async Task ExecuteOperationsAsync()
        {
            IEnumerable<Rule> rulesIntermediateResult = rules;

            foreach (IManagementOperation managementOperation in this.managementOperations)
            {
                rulesIntermediateResult = await managementOperation.ApplyAsync(rulesIntermediateResult).ConfigureAwait(false);
            }
        }

        public ManagementOperationsController FilterFromThresholdPriorityToBottom(int thresholdPriority)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation(thresholdPriority, null));

        public ManagementOperationsController FilterPrioritiesRange(int topPriorityThreshold, int bottomPriorityThreshold)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation(topPriorityThreshold, bottomPriorityThreshold));

        public ManagementOperationsController IncreasePriority()
            => this.AddOperation(new MovePriorityManagementOperation(1));

        public ManagementOperationsController SetRuleForUpdate(Rule updatedRule)
            => this.AddOperation(new SetRuleForUpdateManagementOperation(updatedRule));

        public ManagementOperationsController UpdateRules()
            => this.AddOperation(new UpdateRulesManagementOperation(this.rulesSource));

        private ManagementOperationsController AddOperation(IManagementOperation managementOperation)
        {
            this.managementOperations.Add(managementOperation);

            return this;
        }
    }
}