namespace Rules.Framework.Admin.UI
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

#if NETSTANDARD2_0

    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
    using Microsoft.AspNetCore.Internal;

    using Microsoft.Extensions.FileProviders;
    using System.Reflection;
    using Rules.Framework.Admin.WebApi;

#endif

    public static class UIBuilderExtensions
    {
        /// <summary>
        /// Register the UI middleware with optional setup action for DI-injected options
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app)
        {
            UIOptions options;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<UIOptions>>().Value;
            }
            return app.UseRulesFrameworkUI(options);
        }

        /// <summary>
        /// Register the UI middleware with provided options
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app, UIOptions options)
        {
            return app.UseMiddleware<UIMiddleware>(options);
        }
    }
}