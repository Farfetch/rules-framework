namespace Rules.Framework.UI.Handlers
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal class GetPriorityCriteriasHandler : UIRequestHandlerBase
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
            var priorityCriterias = this.rulesEngine.GetPriorityCriterias();

            return this.WriteResponseAsync(httpResponse, priorityCriterias.ToString(), (int)HttpStatusCode.OK);
        }
    }
}