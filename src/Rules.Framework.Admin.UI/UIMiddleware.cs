namespace Rules.Framework.Admin.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Hosting;

#if NETSTANDARD2_0

    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
    using System;

#endif

    public class UIMiddleware
    {
        private readonly UIOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;

        public UIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            UIOptions options)
        {
            _options = options ?? new UIOptions();

            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                // Use relative redirect to support proxy environments
                var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                    ? "index.html"
                    : $"{path.Split('/').Last()}/index.html";

                RespondWithRedirect(httpContext.Response, relativeIndexUrl);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            UIOptions options)
        {
            var asssembly = typeof(UIMiddleware).GetTypeInfo().Assembly;

            var provider = new EmbeddedFileProvider(asssembly, asssembly.GetName().Name + ".node_modules.glyphicons_only_bootstrap");

            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
                FileProvider = provider
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", _options.DocumentTitle },
                { "%(HeadContent)", _options.HeadContent }
            };
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            using (var stream = _options.IndexStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    // Inject arguments before writing to response
                    var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
                    foreach (var entry in GetIndexArguments())
                    {
                        htmlBuilder.Replace(entry.Key, entry.Value);
                    }

                    await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }
    }
}