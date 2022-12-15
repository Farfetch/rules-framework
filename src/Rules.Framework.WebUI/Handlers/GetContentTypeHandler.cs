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
        private static readonly string[] resourcePath = new[] { "/rules/ContentType/List" };

        private readonly IGenericRulesEngine genericRulesEngineAdapter;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetContentTypeHandler(IGenericRulesEngine rulesEngine, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer) : base(resourcePath)
        {
            this.genericRulesEngineAdapter = rulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var contents = this.genericRulesEngineAdapter.GetContentTypes();

                var contentTypes = new List<ContentTypeDto>();

                foreach (var identifier in contents.Select(c => c.Identifier))
                {
                    var genericSearchArgs = new GenericContentType { Identifier = identifier };

                    var genericRules = await this.genericRulesEngineAdapter
                        .SearchAsync(new SearchArgs<GenericContentType, GenericConditionType>(genericSearchArgs,
                            DateTime.MinValue,
                            DateTime.MaxValue))
                        .ConfigureAwait(false);

                    contentTypes.Add(new ContentTypeDto
                    {
                        Name = identifier,
                        ActiveRulesCount = genericRules.Count(IsActive),
                        RulesCount = genericRules.Count()
                    });
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