namespace Rules.Framework.WebUI
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Utitlies;

    internal abstract class WebUIRequestHandlerBase : IHttpRequestHandler
    {
        protected readonly JsonSerializerOptions SerializerOptions;
        protected readonly WebUIOptions WebUIOptions;

        protected WebUIRequestHandlerBase(string[] resourcePath, WebUIOptions webUIOptions)
        {
            this.ResourcePath = resourcePath;
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

        protected abstract HttpMethod HttpMethod { get; }

        protected string[] ResourcePath { get; }

        public virtual async Task<bool> HandleAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            if (!this.CanHandle(httpRequest))
            {
                return false;
            }

            await this.HandleRequestAsync(httpRequest, httpResponse, next).ConfigureAwait(false);

            return true;
        }

        protected bool CanHandle(HttpRequest httpRequest)
        {
            var resource = httpRequest.Path.ToUriComponent();

            var resourcesPath = this.ResourcePath.Select(r => string.Format(r, this.WebUIOptions.RoutePrefix));

            if (!resourcesPath.Contains(resource))
            {
                return false;
            }

            var method = httpRequest.Method;

            if (!method.Equals(this.HttpMethod.ToString()))
            {
                return false;
            }

            return true;
        }

        protected abstract Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next);

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
    }
}
