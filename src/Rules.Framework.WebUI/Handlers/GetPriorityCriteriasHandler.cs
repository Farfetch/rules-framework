namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal class GetPriorityCriteriasHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules/Rule/Priority" };

        private readonly IRulesEngine rulesEngine;

        public GetPriorityCriteriasHandler(IRulesEngine rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            try
            {
                var priorityCriterias = this.rulesEngine.GetPriorityCriterias();

                return this.WriteResponseAsync(httpResponse, priorityCriterias.ToString(), (int)HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                return this.WriteResponseAsync(httpResponse, ex.Message.ToString() + Environment.NewLine + ex.InnerException.ToString(), (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}