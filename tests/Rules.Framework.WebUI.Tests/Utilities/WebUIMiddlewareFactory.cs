namespace Rules.Framework.WebUI.Tests.Utilities
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public static class WebUIMiddlewareFactory
    {
        internal static WebUIMiddleware Create(
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory)
        {
            return new WebUIMiddleware(loggerFactory: loggerFactory, hostingEnv: hostingEnv,
                next: (_) =>
                {
                    return Task.CompletedTask;
                },
                options: new WebUIOptions());
        }
    }
}