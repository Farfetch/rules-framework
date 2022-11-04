namespace Rules.Framework.WebUI
{
    using Microsoft.AspNetCore.Builder;
    using Rules.Framework.WebUI.Handlers;

    /// <summary>
    /// IApplicationBuilder extension for Rules Framework UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Register the UI middleware
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app, IRulesEngine rulesEngine)
        {
            app.UseMiddleware<WebUIMiddleware>(new GetIndexPageHandler(new WebUIOptions()));
            app.UseMiddleware<WebUIMiddleware>(new GetPriorityCriteriasHandler(rulesEngine));
            app.UseMiddleware<WebUIMiddleware>(new GetContentTypeHandler(rulesEngine));
            app.UseMiddleware<WebUIMiddleware>(new GetRulesHandler(rulesEngine));

            return app;
        }
    }
}