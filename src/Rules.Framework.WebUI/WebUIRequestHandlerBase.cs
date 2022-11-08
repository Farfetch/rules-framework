namespace Rules.Framework.WebUI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    internal abstract class WebUIRequestHandlerBase : IHttpRequestHandler
    {
        protected JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            TypeNameHandling = TypeNameHandling.None
        };

        protected WebUIRequestHandlerBase(string[] resourcePath)
        {
            this.ResourcePath = resourcePath;
            this.jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            this.jsonSerializerSettings.Converters.Add(new StringEnumConverter());
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
            var body = JsonConvert.SerializeObject(responseDto, this.jsonSerializerSettings);
            httpResponse.StatusCode = statusCode;
            httpResponse.ContentType = "application/json";
            httpResponse.Headers.ContentLength = body.Length;

            return httpResponse.WriteAsync(body, Encoding.UTF8);
        }
    }
}