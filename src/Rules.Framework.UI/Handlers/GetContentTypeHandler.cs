namespace Rules.Framework.UI.Handlers
{
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
            var contents = this.rulesEngine.GetContentTypes();

            var list = new List<ContentTypeDto>();

            foreach (var content in contents)
            {
                list.Add(new ContentTypeDto { Name = content.Name });
            }

            return this.WriteResponseAsync(httpResponse, list, (int)HttpStatusCode.OK);
        }
    }
}