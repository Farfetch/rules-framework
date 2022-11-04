namespace Rules.Framework.WebUI.Handlers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

#if NETSTANDARD2_0

    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
    using System;

#endif

    internal class IndexPageHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules", "/rules/index.html" };
        private readonly WebUIOptions options;

        public IndexPageHandler(WebUIOptions options) : base(resourcePath)
        {
            this.options = options;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var path = httpRequest.Path.Value;
            var httpContext = httpRequest.HttpContext;

            if (Regex.IsMatch(path, $"^/?{Regex.Escape(this.options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                // Use relative redirect to support proxy environments
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                this.RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            }

            if (Regex.IsMatch(path, $"^/{Regex.Escape(this.options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await this.RespondWithIndexHtmlAsync(httpContext.Response);
            }
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", this.options.DocumentTitle },
                { "%(HeadContent)", this.options.HeadContent }
            };
        }

        private async Task RespondWithIndexHtmlAsync(HttpResponse httpResponse)
        {
            httpResponse.StatusCode = 200;
            httpResponse.ContentType = "text/html;charset=utf-8";

            using (var stream = this.options.IndexStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var responseTextBuilder = new StringBuilder(await reader.ReadToEndAsync());

                    foreach (var entry in this.GetIndexArguments())
                    {
                        responseTextBuilder.Replace(entry.Key, entry.Value);
                    }

                    await httpResponse.WriteAsync(responseTextBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        private void RespondWithRedirect(HttpResponse httpResponse, string location)
        {
            httpResponse.StatusCode = 301;
            httpResponse.Headers["Location"] = location;
        }
    }
}