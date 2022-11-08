namespace Rules.Framework.WebUI.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text;

#if NETSTANDARD2_0

    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

#endif

    internal class GetIndexPageHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/rules", "/rules/", "/rules/index.html" };
        private readonly WebUIOptions options;

        public GetIndexPageHandler(WebUIOptions options) : base(resourcePath)
        {
            this.options = options;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            var path = httpRequest.Path.Value;
            var httpContext = httpRequest.HttpContext;

            await this.RespondWithIndexHtmlAsync(httpContext.Response, next);
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", this.options.DocumentTitle },
                { "%(HeadContent)", this.options.HeadContent }
            };
        }

        private async Task RespondWithIndexHtmlAsync(HttpResponse httpResponse, RequestDelegate next)
        {
            httpResponse.StatusCode = 200;
            httpResponse.ContentType = "text/html;charset=utf-8";

            var originalBody = httpResponse.Body;

            using (var stream = this.options.IndexStream())
            {
                httpResponse.Body = stream;
                await next(httpResponse.HttpContext);

                using (var reader = new StreamReader(stream))
                {
                    var responseTextBuilder = new StringBuilder(await reader.ReadToEndAsync());

                    foreach (var entry in this.GetIndexArguments())
                    {
                        responseTextBuilder.Replace(entry.Key, entry.Value);
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(responseTextBuilder.ToString());
                    using (var newStream = new MemoryStream(byteArray))
                    {
                        httpResponse.Body = originalBody;
                        newStream.Seek(0, SeekOrigin.Begin);
                        await newStream.CopyToAsync(httpResponse.Body);
                    }
                }

                httpResponse.Body = originalBody;
            }
        }
    }
}