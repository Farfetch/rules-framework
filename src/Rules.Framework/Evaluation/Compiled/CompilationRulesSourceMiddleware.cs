namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Source;

    internal sealed class CompilationRulesSourceMiddleware<TContentType, TConditionType> : IRulesSourceMiddleware<TContentType, TConditionType>
    {
        private readonly IRuleConditionsExpressionBuilder<TConditionType> ruleConditionsExpressionBuilder;
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        public CompilationRulesSourceMiddleware(
            IRuleConditionsExpressionBuilder<TConditionType> ruleConditionsExpressionBuilder,
            IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.ruleConditionsExpressionBuilder = ruleConditionsExpressionBuilder;
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
            var result = await next.Invoke(args).ConfigureAwait(false);

            var rules = result.ToList();

            var compiledRulesTasks = rules.Select(async (rule) =>
            {
                bool compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
                return rule;
            });

            return await Task.WhenAll(compiledRulesTasks).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs<TContentType> args,
            GetRulesFilteredDelegate<TContentType, TConditionType> next)
        {
            var result = await next.Invoke(args).ConfigureAwait(false);

            var rules = result.ToList();

            var compiledRulesTasks = rules.Select(async (rule) =>
            {
                bool compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
                return rule;
            });

            return await Task.WhenAll(compiledRulesTasks).ConfigureAwait(false);
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

            if (conditionNode is { } && (!conditionNode.Properties.TryGetValue(ConditionNodeProperties.CompilationProperties.IsCompiledKey, out var compiledFlag) || !(bool)compiledFlag))
            {
                var expression = this.ruleConditionsExpressionBuilder.BuildExpression(conditionNode);
                var compiledExpression = expression.Compile();
                conditionNode.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = compiledExpression;
                conditionNode.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
                return true;
            }

            return false;
        }
    }
}