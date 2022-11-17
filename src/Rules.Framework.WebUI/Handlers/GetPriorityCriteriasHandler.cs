namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generic;

    internal class GetPriorityCriteriasHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules/Rule/Priority" };

        private readonly IGenericRulesEngineAdapter rulesEngine;

        public GetPriorityCriteriasHandler(IGenericRulesEngineAdapter rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var priorityCriterias = this.rulesEngine.GetPriorityCriterias();

                return this.WriteResponseAsync(httpResponse, priorityCriterias.ToString(), (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }
    }
}