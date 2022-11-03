namespace Rules.Framework.UI
{
    using Microsoft.AspNetCore.Builder;
    using Rules.Framework.UI.Handlers;

    /// <summary>
    /// IApplicationBuilder extension for Rules Framework UI
    /// </summary>
    public static class UIBuilderExtensions
    {
        /// <summary>
        /// Register the UI middleware
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app, IRulesEngine rulesEngine)
        {
            app.UseMiddleware<UIMiddleware>(new IndexPageHandler(new UIOptions()));
            app.UseMiddleware<UIMiddleware>(new GetPriorityCriteriasHandler(rulesEngine));
            app.UseMiddleware<UIMiddleware>(new GetContentTypeHandler(rulesEngine));
            app.UseMiddleware<UIMiddleware>(new GetRulesHandler(rulesEngine));

            return app;
        }
    }
}