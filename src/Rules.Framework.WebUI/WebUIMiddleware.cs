namespace Rules.Framework.WebUI
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal sealed class WebUIMiddleware
    {
        private readonly StaticFileMiddleware staticFileMiddlewares;

        public WebUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            WebUIOptions options)
        {
            this.staticFileMiddlewares = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options, ".node_modules");
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await this.ExecuteStaticFileMiddlewareAsync(httpContext).ConfigureAwait(true);
        }

        private static StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            WebUIOptions options,
            string path)
        {
            var asssembly = typeof(WebUIMiddleware).GetTypeInfo().Assembly;

            var provider = new EmbeddedFileProvider(asssembly, asssembly.GetName().Name + path);

            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
                FileProvider = provider,
                ServeUnknownFileTypes = true
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }

        private Task ExecuteStaticFileMiddlewareAsync(HttpContext httpContext)
        {
            return this.staticFileMiddlewares
                .Invoke(httpContext);
        }
    }
}