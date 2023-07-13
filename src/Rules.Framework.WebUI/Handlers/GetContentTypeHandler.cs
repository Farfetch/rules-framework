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
                var contents = this.genericRulesEngine.GetContentTypes();

                var contentTypes = new List<ContentTypeDto>();
                var index = 0;
                foreach (var identifier in contents.Select(c => c.Identifier))
                {
                    var genericContentType = new GenericContentType { Identifier = identifier };

                    var genericRules = await this.genericRulesEngine
                        .SearchAsync(new SearchArgs<GenericContentType, GenericConditionType>(genericContentType,
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

        private bool IsActive(GenericRule genericRule)
        {
            return this.ruleStatusDtoAnalyzer.Analyze(genericRule.DateBegin, genericRule.DateEnd) == RuleStatusDto.Active;
        }
    }
}
