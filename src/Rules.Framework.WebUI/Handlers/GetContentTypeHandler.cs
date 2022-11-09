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

        private readonly IGenericRulesEngine rulesEngine;

        public GetContentTypeHandler(IGenericRulesEngine rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var contents = this.rulesEngine.GetContentTypes();

                var contentTypes = new List<ContentTypeDto>();

                foreach (var content in contents)
                {
                    var genericRules = await this.rulesEngine.SearchAsync(
                    new SearchArgs<GenericContentType, GenericConditionType>(
                        new GenericContentType { Name = content.Name }, DateTime.MinValue, DateTime.MaxValue)
                    );

                    contentTypes.Add(new ContentTypeDto
                    {
                        Name = content.Name,
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
            if (genericRule.DateBegin > DateTime.UtcNow)
            {
                return false;
            }

            if (genericRule.DateEnd is null)
            {
                return true;
            }

            if (genericRule.DateEnd <= DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
    }
}