namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Core;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;

    internal sealed class GetContentTypeHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/contentTypes" };

        private readonly IGenericRulesEngine genericRulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetContentTypeHandler(IGenericRulesEngine genericRulesEngine,
            IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer,
            WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.genericRulesEngine = genericRulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var contentTypes = this.genericRulesEngine.GetContentTypes();

                var contentTypeDtos = new List<ContentTypeDto>();
                var index = 0;
                foreach (var contentType in contentTypes)
                {
                    var genericRules = await this.genericRulesEngine
                        .SearchAsync(new SearchArgs<string, string>(contentType,
                            DateTime.MinValue,
                            DateTime.MaxValue))
                        .ConfigureAwait(false);

                    contentTypeDtos.Add(new ContentTypeDto
                    {
                        Index = index,
                        Name = contentType,
                        ActiveRulesCount = genericRules.Count(IsActive),
                        RulesCount = genericRules.Count()
                    });
                    index++;
                }

                await this.WriteResponseAsync(httpResponse, contentTypeDtos, (int)HttpStatusCode.OK).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }

        private bool IsActive(Rule<string, string> genericRule)
        {
            return this.ruleStatusDtoAnalyzer.Analyze(genericRule.DateBegin, genericRule.DateEnd) == RuleStatusDto.Active;
        }
    }
}