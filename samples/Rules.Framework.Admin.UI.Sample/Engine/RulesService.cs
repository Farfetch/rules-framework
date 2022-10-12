namespace Rules.Framework.Admin.UI.Sample.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Rules.Framework.Admin.UI.Sample.Enums;
    using global::Rules.Framework.Admin.UI.Sample.Exceptions;
    using global::Rules.Framework.Admin.WebApi;

    public class RulesService : IRulesService
    {
        private readonly RulesEngineProvider rulesEngineProvider;

        public RulesService(IEnumerable<IContentTypes> contentTypes)
        {
            this.rulesEngineProvider = new RulesEngineProvider(new RulesBuilder(contentTypes));
        }

        public async Task<List<dynamic>> FindRulesAsync(string contentType, DateTime dateTime)
        {
            var rulesEngine = await
                rulesEngineProvider
                .GetRulesEngineAsync()
            .ConfigureAwait(false);

            if (Enum.TryParse(contentType, out ContentTypes contentTypes))
            {
                var list = await rulesEngine
                .SearchAsync(new SearchArgs<ContentTypes, ConditionTypes>(contentTypes, dateTime, dateTime))
                .ConfigureAwait(false);

                return list.OrderBy(d => d.Priority).ToList<dynamic>();
            }

            return new List<dynamic>();
        }

        public IEnumerable<string> ListContents()
        {
            return Enum.GetNames(typeof(ContentTypes)).ToList();
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