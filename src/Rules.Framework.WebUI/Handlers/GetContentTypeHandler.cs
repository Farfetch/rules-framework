namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
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

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var contents = this.rulesEngine.GetContentTypes();

                var contentTypes = new List<ContentTypeDto>();

                foreach (var content in contents)
                {
                    contentTypes.Add(new ContentTypeDto { Name = content.Name });
                }

                return this.WriteResponseAsync(httpResponse, contentTypes, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }
    }
}