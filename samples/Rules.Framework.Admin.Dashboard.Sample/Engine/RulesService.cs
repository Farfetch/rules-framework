namespace Rules.Framework.Admin.Dashboard.Sample.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Rules.Framework.Admin.Dashboard.Sample.Enums;
    using global::Rules.Framework.Admin.Dashboard.Sample.Exceptions;

    public class RulesService
    {
        private readonly RulesEngineProvider rulesEngineProvider;

        public RulesService(IEnumerable<IContentTypes> contentTypes)
        {
            this.rulesEngineProvider = new RulesEngineProvider(new RulesBuilder(contentTypes));
        }

        public async Task<T> MatchOneAsync<T>(
            ContentTypes contentType,
            DateTime dateTime,
            IDictionary<ConditionTypes, object> conditions)
        {
            var rulesConditions = (conditions is null) ? new Condition<ConditionTypes>[] { } :
                conditions.Select(x => new Condition<ConditionTypes> { Type = x.Key, Value = x.Value })
                .ToArray();

            var rulesEngine = await
                rulesEngineProvider
                .GetRulesEngineAsync()
                .ConfigureAwait(false);

            var match = await rulesEngine
                .MatchOneAsync(contentType, dateTime, rulesConditions)
                .ConfigureAwait(false);

            if (match is null)
            {
                var message = $"Error trying to match one rule. No rule was found {contentType} {dateTime} {string.Join(", ", conditions)}.";

                throw new RulesNotFoundException(message);
            }

            return match.ContentContainer.GetContentAs<T>();
        }
    }
}