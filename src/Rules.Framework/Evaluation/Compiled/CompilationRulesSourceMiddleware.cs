namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using Rules.Framework.Source;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;

    internal class CompilationRulesSourceMiddleware<TContentType, TConditionType> : IRulesSourceMiddleware<TContentType, TConditionType>
    {
        private readonly IConditionsTreeCompiler<TConditionType> conditionsTreeCompiler;
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        public CompilationRulesSourceMiddleware(
            IConditionsTreeCompiler<TConditionType> conditionsTreeCompiler,
            IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.conditionsTreeCompiler = conditionsTreeCompiler;
            this.rulesDataSource = rulesDataSource;
        }

        public async Task HandleAddRuleAsync(
            AddRuleArgs<TContentType, TConditionType> args,
            AddRuleDelegate<TContentType, TConditionType> next)
        {
            this.TryCompile(args.Rule);

            await next.Invoke(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesAsync(
            GetRulesArgs<TContentType> args,
            GetRulesDelegate<TContentType, TConditionType> next)
        {
            var rules = await next.Invoke(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                bool compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
            }

            return rules;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs<TContentType> args,
            GetRulesFilteredDelegate<TContentType, TConditionType> next)
        {
            var rules = await next.Invoke(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                bool compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
            }

            return rules;
        }

        public async Task HandleUpdateRuleAsync(
            UpdateRuleArgs<TContentType, TConditionType> args,
            UpdateRuleDelegate<TContentType, TConditionType> next)
        {
            this.TryCompile(args.Rule);

            await next.Invoke(args).ConfigureAwait(false);
        }

        private bool TryCompile(Rule<TContentType, TConditionType> rule)
        {
            var conditionNode = rule.RootCondition;

            if (conditionNode is { } && (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompiledFlagKey, out var compiledFlag) || !(bool)compiledFlag))
            {
                this.conditionsTreeCompiler.Compile(conditionNode);
                conditionNode.Properties[ConditionNodeProperties.CompiledFlagKey] = true;
                return true;
            }

            return false;
        }
    }
}
