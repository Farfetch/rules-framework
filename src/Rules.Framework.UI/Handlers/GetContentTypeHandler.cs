namespace Rules.Framework.UI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.UI.Dto;

    internal class GetContentTypeHandler : UIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules/ContentType/List" };

        private readonly IRulesEngine rulesEngine;

        public GetContentTypeHandler(IRulesEngine rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            try
            {
                var contents = this.rulesEngine.GetContentTypes();

                var list = new List<ContentTypeDto>();

                foreach (var content in contents)
                {
                    list.Add(new ContentTypeDto { Name = content.Name });
                }

                return this.WriteResponseAsync(httpResponse, list, (int)HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                return this.WriteResponseAsync(httpResponse, ex.Message.ToString() + Environment.NewLine + ex.InnerException.ToString(), (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}