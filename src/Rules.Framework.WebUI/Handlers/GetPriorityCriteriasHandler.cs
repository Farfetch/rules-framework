namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;

    internal sealed class GetPriorityCriteriaHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules/Rule/Priority" };

        private readonly IGenericRulesEngine rulesEngine;

        public GetPriorityCriteriaHandler(IGenericRulesEngine rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var priorityCriteria = this.rulesEngine.GetPriorityCriteria();

                return this.WriteResponseAsync(httpResponse, priorityCriteria.ToString(), (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }
    }
}