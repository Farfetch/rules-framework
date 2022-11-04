namespace Rules.Framework.WebUI
{
    using System.Reflection;
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

    internal sealed class WebUIMiddleware
    {
        private readonly IHttpRequestHandler httpRequestHandler;
        private readonly RequestDelegate next;
        private readonly WebUIOptions options;
        private readonly StaticFileMiddleware staticFileMiddlewares;

        public WebUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            IHttpRequestHandler httpRequestHandler)
        {
            this.options = new WebUIOptions();

            this.staticFileMiddlewares = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, this.options, ".node_modules");
            this.httpRequestHandler = httpRequestHandler;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await this.staticFileMiddlewares
                .Invoke(httpContext)
                .ConfigureAwait(false);

            var handled = await this.httpRequestHandler
                .HandleAsync(httpContext.Request, httpContext.Response)
                .ConfigureAwait(false);

            if (!handled)
            {
                await this.next(httpContext);
            }
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next,
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
                FileProvider = provider
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }
    }
}