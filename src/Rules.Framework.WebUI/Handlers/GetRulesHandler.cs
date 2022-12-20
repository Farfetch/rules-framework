namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;

    internal sealed class GetRulesHandler : WebUIRequestHandlerBase
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/rules" };
        private readonly IGenericRulesEngine rulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetRulesHandler(IGenericRulesEngine rulesEngine, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer, WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.rulesEngine = rulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            if (!httpRequest.Query.TryGetValue("contentType", out var contentTypeName))
            {
                await this.WriteResponseAsync(httpResponse, new { Message = "contentType is required" }, (int)HttpStatusCode.BadRequest)
                    .ConfigureAwait(false);

                return;
            }

            try
            {
                var genericRules = await this.rulesEngine.SearchAsync(
                    new SearchArgs<GenericContentType, GenericConditionType>(
                        new GenericContentType { Identifier = contentTypeName },
                        DateTime.MinValue, DateTime.MaxValue))
                    .ConfigureAwait(false);

                var rules = new List<RuleDto>(genericRules.Count());

                var priorityCriteria = this.rulesEngine.GetPriorityCriteria();

                if (genericRules != null && genericRules.Any())
                {
                    if (priorityCriteria == PriorityCriterias.BottommostRuleWins)
                    {
                        genericRules = genericRules.OrderByDescending(r => r.Priority);
                    }
                    else
                    {
                        genericRules = genericRules.OrderBy(r => r.Priority);
                    }

                    foreach (var rule in genericRules)
                    {
                        rules.Add(new RuleDto
                        {
                            Priority = rule.Priority,
                            Name = rule.Name,
                            Value = rule.Content,
                            DateEnd = !rule.DateEnd.HasValue ? null : rule.DateEnd.Value.ToString(dateFormat),
                            DateBegin = rule.DateBegin.ToString(dateFormat),
                            Status = this.ruleStatusDtoAnalyzer.Analyze(rule.DateBegin, rule.DateEnd).ToString(),
                            Conditions = rule.RootCondition
                        });
                    }
                }

                await this.WriteResponseAsync(httpResponse, rules, (int)HttpStatusCode.OK).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }
    }
}