namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;
    using Rules.Framework.Rql;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Extensions;

    internal class PostRqlHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePaths = new[] { "api/v1/rql" };
        private readonly IGenericRulesEngine genericRulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public PostRqlHandler(IGenericRulesEngine genericRulesEngine, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer, WebUIOptions webUIOptions)
            : base(resourcePaths, webUIOptions)
        {
            this.genericRulesEngine = genericRulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override async Task HandleAsync(HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;
            var request = await JsonSerializer.DeserializeAsync<RqlInputDto>(utf8Json: httpRequest.Body).ConfigureAwait(false);
            try
            {
                var textWriter = new StringWriter();
                var genericRqlOptions = new GenericRqlOptions
                {
                    OutputWriter = textWriter,
                };

                using (var genericRqlClient = this.genericRulesEngine.GetRqlClient(genericRqlOptions))
                {
                    var genericRqlResult = await genericRqlClient.ExecuteAsync(request.Rql).ConfigureAwait(false);
                    var response = genericRqlResult.ToRqlOutput(this.ruleStatusDtoAnalyzer, textWriter.GetStringBuilder().ToString());
                    await this.WriteResponseAsync(httpResponse, response, (int)HttpStatusCode.OK).ConfigureAwait(false);
                }
            }
            catch (RqlException rqlException)
            {
                var response = rqlException.ToRqlOutput();
                await this.WriteResponseAsync(httpResponse, response, (int)HttpStatusCode.BadRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }
    }
}