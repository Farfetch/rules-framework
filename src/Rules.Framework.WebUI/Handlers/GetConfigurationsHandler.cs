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
        private static readonly string[] resourcePath = new[] { "api/v1/configurations" };

        private readonly IGenericRulesEngine rulesEngine;

        public GetConfigurationsHandler(IGenericRulesEngine rulesEngine, WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.rulesEngine = rulesEngine;
        }

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override async Task HandleAsync(HttpContext httpContext)
        {
            var httpResponse = httpContext.Response;
            try
            {
                var priorityCriteria = this.rulesEngine.GetPriorityCriteria();

                var configurations = new Dictionary<string, string>
                {
                    { "PriorityCriteria", priorityCriteria.ToString() }
                };

                await this.WriteResponseAsync(httpResponse, configurations, (int)HttpStatusCode.OK).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }
    }
}