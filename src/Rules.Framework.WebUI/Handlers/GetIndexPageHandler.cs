namespace Rules.Framework.WebUI.Handlers
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using RazorLight;

    internal sealed class GetIndexPageHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePaths = new[] { "", "index.html" };
        private readonly IRazorLightEngine razorLightEngine;

        public GetIndexPageHandler(IRazorLightEngine razorLightEngine, WebUIOptions webUIOptions)
            : base(resourcePaths, webUIOptions)
        {
            this.razorLightEngine = razorLightEngine;
        }

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override async Task HandleAsync(HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            var path = httpRequest.Path.Value;

            if (Regex.IsMatch(path, $"^/?{Regex.Escape(this.WebUIOptions.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                // Use relative redirect to support proxy environments
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            }

            if (Regex.IsMatch(path, $"^/{Regex.Escape(this.WebUIOptions.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await this.RespondWithIndexHtmlAsync(httpContext.Response).ConfigureAwait(false);
            }
        }

        private static void RespondWithRedirect(HttpResponse httpResponse, string location)
        {
            if (!httpResponse.HasStarted)
            {
                httpResponse.StatusCode = 301;
                httpResponse.Headers["Location"] = location;
            }
        }

        private object GetIndexArguments()
        {
            return new
            {
                DocumentTitle = this.WebUIOptions.DocumentTitle,
                HeadContent = this.WebUIOptions.HeadContent,
            };
        }

        private async Task RespondWithIndexHtmlAsync(HttpResponse httpResponse)
        {
            if (!httpResponse.HasStarted)
            {
                httpResponse.StatusCode = 200;
                httpResponse.ContentType = "text/html;charset=utf-8";

                var originalBody = httpResponse.Body;

                using (var stream = this.WebUIOptions.IndexStream())
                {
                    httpResponse.Body = stream;
                    using (var reader = new StreamReader(stream))
                    {
                        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
                        string result;
                        var cacheResult = this.razorLightEngine.Handler.Cache.RetrieveTemplate("index");
                        if (cacheResult.Success)
                        {
                            var templatePage = cacheResult.Template.TemplatePageFactory.Invoke();
                            result = await this.razorLightEngine.RenderTemplateAsync(templatePage, this.GetIndexArguments()).ConfigureAwait(false);
                        }
                        else
                        {
                            result = await this.razorLightEngine.CompileRenderStringAsync("index", body, this.GetIndexArguments()).ConfigureAwait(false);
                        }

                        var byteArray = Encoding.UTF8.GetBytes(result);
                        using (var newStream = new MemoryStream(byteArray))
                        {
                            httpResponse.Body = originalBody;
                            newStream.Seek(0, SeekOrigin.Begin);
                            await newStream.CopyToAsync(httpResponse.Body).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }
}