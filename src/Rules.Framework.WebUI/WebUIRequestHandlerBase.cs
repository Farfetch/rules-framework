namespace Rules.Framework.WebUI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal abstract class WebUIRequestHandlerBase : IHttpRequestHandler
    {
        /*protected JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            TypeNameHandling = TypeNameHandling.None
        };*/

        protected JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IncludeFields = true
        };

        protected WebUIRequestHandlerBase(string[] resourcePath)
        {
            this.ResourcePath = resourcePath;
            this.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

            if (!this.ResourcePath.Contains(resource))
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
            var error = exception.Message.ToString();
            if (exception.InnerException != null)
            {
                error += Environment.NewLine + exception.InnerException.ToString();
            }

            return this.WriteResponseAsync(httpResponse, error, (int)HttpStatusCode.InternalServerError);
        }

        protected virtual Task WriteResponseAsync<T>(HttpResponse httpResponse, T responseDto, int statusCode)
        {
            if (!httpResponse.HasStarted)
            {
                var body = JsonSerializer.Serialize(responseDto, this.SerializerOptions);
                httpResponse.StatusCode = statusCode;
                httpResponse.ContentType = "application/json";
                httpResponse.Headers.ContentLength = body.Length;

                return httpResponse.WriteAsync(body, Encoding.UTF8);
            }
            else
            {
                return httpResponse.WriteAsync(string.Empty);
            }
        }
    }
}