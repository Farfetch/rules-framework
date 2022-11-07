namespace Rules.Framework.WebUI.Tests.Utilities
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

    public static class WebUIMiddlewareFactory
    {
        internal static WebUIMiddleware Create(
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            IHttpRequestHandler handler)
        {
            return new WebUIMiddleware(loggerFactory: loggerFactory, hostingEnv: hostingEnv,
                next: (innerHttpContext) =>
                {
                    return Task.CompletedTask;
                },
                httpRequestHandler: handler
                );
        }
    }
}