namespace Rules.Framework.WebUI
{
    using System;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Utilities;

    internal abstract class WebUIRequestHandlerBase : IHttpRequestHandler
    {
        protected readonly JsonSerializerOptions SerializerOptions;
        protected readonly WebUIOptions WebUIOptions;

        protected WebUIRequestHandlerBase(string[] resourcePaths, WebUIOptions webUIOptions)
        {
            this.ResourcePaths = PreProcessResourcePaths(resourcePaths, webUIOptions);
            this.WebUIOptions = webUIOptions;
            this.SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            this.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            this.SerializerOptions.Converters.Add(new PolymorphicWriteOnlyJsonConverter<ConditionNodeDto>());
        }

        public abstract HttpMethod HttpMethod { get; }

        public string[] ResourcePaths { get; }

        public abstract Task HandleAsync(HttpContext httpContext);

        protected Task WriteExceptionResponseAsync(HttpResponse httpResponse, Exception exception)
        {
            var error = new StringBuilder(exception.Message);
            if (exception.InnerException != null)
            {
                error.AppendLine(exception.InnerException.Message);
            }

            return this.WriteResponseAsync(httpResponse, error, (int)HttpStatusCode.InternalServerError);
        }

        protected virtual async Task WriteResponseAsync<T>(HttpResponse httpResponse, T responseDto, int statusCode)
        {
            if (!httpResponse.HasStarted)
            {
                var body = JsonSerializer.Serialize(responseDto, this.SerializerOptions);
                httpResponse.StatusCode = statusCode;
                httpResponse.ContentType = "application/json";
                httpResponse.Headers.ContentLength = body.Length;

                await httpResponse.WriteAsync(body, Encoding.UTF8).ConfigureAwait(false);
            }
        }

        private static string[] PreProcessResourcePaths(string[] resourcePaths, WebUIOptions webUIOptions)
        {
            var prefix = string.Empty;
            if (!string.IsNullOrEmpty(webUIOptions.RoutePrefix))
            {
                prefix = $"{webUIOptions?.RoutePrefix}/";
            }

            var processedResourcePaths = new string[resourcePaths.Length];
            for (int i = 0; i < resourcePaths.Length; i++)
            {
                processedResourcePaths[i] = $"{prefix}{resourcePaths[i]}";
            }

            return processedResourcePaths;
        }
    }
}