namespace Rules.Framework.Admin.UI
{
    using Microsoft.AspNetCore.Builder;

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
        /// Register the UI middleware
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UIMiddleware>();
        }
    }
}