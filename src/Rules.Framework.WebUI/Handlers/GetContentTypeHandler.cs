namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generic;
    using Rules.Framework.WebUI.Dto;

    internal class GetContentTypeHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules/ContentType/List" };

        private readonly IGenericRulesEngineAdapter rulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetContentTypeHandler(IGenericRulesEngineAdapter rulesEngine, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var contents = this.rulesEngine.GetContentTypes();

                var contentTypes = new List<ContentTypeDto>();

                foreach (var contentName in contents.Select(content => content.Name))
                {
                    var genericRules = await this.rulesEngine.SearchAsync(
                    new SearchArgs<GenericContentType, GenericConditionType>(
                        new GenericContentType { Name = contentName }, DateTime.MinValue, DateTime.MaxValue)
                    );

                    contentTypes.Add(new ContentTypeDto
                    {
                        Name = contentName,
                        ActiveRulesCount = genericRules.Count(IsActive),
                        RulesCount = genericRules.Count()
                    });
                }

                await this.WriteResponseAsync(httpResponse, contentTypes, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }

        private bool IsActive(GenericRule genericRule)
        {
            return this.ruleStatusDtoAnalyzer.Analyze(genericRule.DateBegin, genericRule.DateEnd) == RuleStatusDto.Active;
        }
    }
}