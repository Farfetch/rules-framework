namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Source;

    internal sealed class CompilationRulesSourceMiddleware : IRulesSourceMiddleware
    {
        private readonly IRuleConditionsExpressionBuilder ruleConditionsExpressionBuilder;
        private readonly IRulesDataSource rulesDataSource;

        public CompilationRulesSourceMiddleware(
            IRuleConditionsExpressionBuilder ruleConditionsExpressionBuilder,
            IRulesDataSource rulesDataSource)
        {
            this.ruleConditionsExpressionBuilder = ruleConditionsExpressionBuilder;
            this.rulesDataSource = rulesDataSource;
        }

        public async Task HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next)
        {
            this.TryCompile(args.Rule);

            await next.Invoke(args).ConfigureAwait(false);
        }

        public Task HandleCreateRulesetAsync(CreateRulesetArgs args, CreateRulesetDelegate next) => next.Invoke(args);

        public async Task<IEnumerable<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next)
        {
            var rules = await next.Invoke(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                var compiled = this.TryCompile(rule);
                if (compiled)
                {
                    // Commit compilation result to data source, so that next time rule is loaded,
                    // it won't go through the compilation process again.
                    await this.rulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);
                }
            }

            return rules;
        }

        public Task<IEnumerable<Ruleset>> HandleGetRulesetsAsync(GetRulesetsArgs args, GetRulesetsDelegate next) => next.Invoke(args);

        public async Task<IEnumerable<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next)
        {
            var rules = await next.Invoke(args).ConfigureAwait(false);

            foreach (var rule in rules)
            {
                var compiled = this.TryCompile(rule);
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
            UpdateRuleArgs args,
            UpdateRuleDelegate next)
        {
            this.TryCompile(args.Rule);

            await next.Invoke(args).ConfigureAwait(false);
        }

        private bool TryCompile(Rule rule)
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