namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.WebUI.Dto;

    internal sealed class GetContentTypeHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/contentTypes" };

        private readonly IRulesEngine rulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetContentTypeHandler(IRulesEngine rulesEngine,
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
                var contents = await this.rulesEngine.GetContentTypesAsync().ConfigureAwait(false);

                var contentTypes = new List<ContentTypeDto>();
                var index = 0;
                foreach (var identifier in contents)
                {
                    var genericRules = await this.rulesEngine
                        .SearchAsync(new SearchArgs<string, string>(identifier,
                            DateTime.MinValue,
                            DateTime.MaxValue))
                        .ConfigureAwait(false);

                    contentTypes.Add(new ContentTypeDto
                    {
                        Index = index,
                        Name = identifier,
                        ActiveRulesCount = genericRules.Count(IsActive),
                        RulesCount = genericRules.Count()
                    });
                    index++;
                }

                await this.WriteResponseAsync(httpResponse, contentTypes, (int)HttpStatusCode.OK).ConfigureAwait(false);
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