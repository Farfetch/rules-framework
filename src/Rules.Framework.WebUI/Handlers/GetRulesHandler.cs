namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Extensions;

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

                var rules = Enumerable.Empty<RuleDto>();

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

                    rules = genericRules.Select(g => g.ToRuleDto(this.ruleStatusDtoAnalyzer));
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