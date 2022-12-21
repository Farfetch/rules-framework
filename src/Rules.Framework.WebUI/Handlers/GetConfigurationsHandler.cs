namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;

    internal sealed class GetConfigurationsHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/configurations" };

        private readonly IGenericRulesEngine rulesEngine;

        public GetConfigurationsHandler(IGenericRulesEngine rulesEngine, WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var priorityCriteria = this.rulesEngine.GetPriorityCriteria();

                var configurations = new Dictionary<string, string>
                {
                    { "PriorityCriteria", priorityCriteria.ToString() }
                };

                return this.WriteResponseAsync(httpResponse, configurations, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }
    }
}