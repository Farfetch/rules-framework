namespace Rules.Framework.WebUI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

#if NETSTANDARD2_0

    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

#endif

    internal sealed class WebUIMiddleware
    {
        private readonly IEnumerable<IHttpRequestHandler> httpRequestHandlers;
        private readonly RequestDelegate next;
        private readonly StaticFileMiddleware staticFileMiddlewares;

        public WebUIMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            IEnumerable<IHttpRequestHandler> httpRequestHandlers,
            WebUIOptions options)
        {
            this.httpRequestHandlers = httpRequestHandlers;
            this.next = next;
            this.staticFileMiddlewares = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options, ".node_modules");
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var anyHandlerExecuted = await this.ExecuteHandlersAsync(httpContext).ConfigureAwait(false);
            if (!anyHandlerExecuted)
            {
                await this.ExecuteStaticFileMiddlewareAsync(httpContext).ConfigureAwait(true);
            }
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

        private async Task<bool> ExecuteHandlersAsync(HttpContext httpContext)
        {
            var results = this.httpRequestHandlers.Select(d => d
                .HandleAsync(httpContext.Request, httpContext.Response, this.next));

            var handle = await Task.WhenAll(results);

            if (handle.All(d => !d))
            {
                await this.next(httpContext);
                return false;
            }
            return true;
        }

        private Task ExecuteStaticFileMiddlewareAsync(HttpContext httpContext)
        {
            return this.staticFileMiddlewares
                .Invoke(httpContext);
        }
    }
}