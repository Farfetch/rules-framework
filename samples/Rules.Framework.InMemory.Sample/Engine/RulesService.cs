namespace Rules.Framework.InMemory.Sample.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Rules.Framework.InMemory.Sample.Enums;
    using global::Rules.Framework.InMemory.Sample.Exceptions;

    internal class RulesService
    {
        private readonly RulesEngineProvider rulesEngineProvider;

        public RulesService(IEnumerable<IRuleSpecificationsProvider> ruleSpecificationsProviders)
        {
            this.rulesEngineProvider = new RulesEngineProvider(new RulesBuilder(ruleSpecificationsProviders));
        }

        public async Task<T> MatchOneAsync<T>(
            RulesetNames ruleset,
            DateTime dateTime,
            IDictionary<ConditionNames, object> conditions)
        {
            var rulesEngine = await
                rulesEngineProvider
                .GetRulesEngineAsync()
                .ConfigureAwait(false);

            var match = await rulesEngine
                .MakeGeneric<RulesetNames, ConditionNames>()
                .MatchOneAsync(ruleset, dateTime, conditions)
                .ConfigureAwait(false);

            if (match is null)
            {
                var message = $"Error trying to match one rule. No rule was found {ruleset} {dateTime} {string.Join(", ", conditions)}.";

                throw new RulesNotFoundException(message);
            }

            return match.ContentContainer.GetContentAs<T>();
        }
    }
}