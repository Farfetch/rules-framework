namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.WebUI.Dto;

    internal sealed class GetRulesetsHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/rulesets" };

        private readonly IRulesEngine rulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetRulesetsHandler(IRulesEngine rulesEngine,
            IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer,
            WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.rulesEngine = rulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var rulesets = await this.rulesEngine.GetRulesetsAsync().ConfigureAwait(false);

                var rulesetDtos = new List<RulesetDto>();
                var index = 0;
                foreach (var ruleset in rulesets)
                {
                    var rules = await this.rulesEngine
                        .SearchAsync(new SearchArgs<string, string>(ruleset.Name,
                            DateTime.MinValue,
                            DateTime.MaxValue))
                        .ConfigureAwait(false);

                    rulesetDtos.Add(new RulesetDto
                    {
                        Index = index,
                        Name = ruleset.Name,
                        ActiveRulesCount = rules.Count(IsActive),
                        RulesCount = rules.Count()
                    });
                    index++;
                }

                await this.WriteResponseAsync(httpResponse, rulesetDtos, (int)HttpStatusCode.OK).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }

        private bool IsActive(Rule rule)
        {
            return this.ruleStatusDtoAnalyzer.Analyze(rule.DateBegin, rule.DateEnd) == RuleStatusDto.Active;
        }
    }
}