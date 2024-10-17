namespace Rules.Framework.WebUI
{
    using System;
    using Components;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Rules.Framework.WebUI.Services;

    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for Rules Framework Web UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app)
        {
            return app.UseRulesFrameworkWebUI(options => { });
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="webUIOptionsAction">The web UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            Action<WebUIOptions> webUIOptionsAction)
        {
            var rulesEngineInstanceProvider = app.ApplicationServices.GetRequiredService<RulesEngineInstanceProvider>();
            rulesEngineInstanceProvider.EnumerateInstances(app.ApplicationServices);

            // Blazor
            var embeddedProvider = new EmbeddedFileProvider(typeof(WebUIApplicationBuilderExtensions).Assembly, "Rules.Framework.WebUI.Assets");

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = embeddedProvider,
                RequestPath = new PathString("/rules-ui")
            });

            app.UseEndpoints(builder =>
            {
                builder.MapRazorComponents<WebUIApp>()
                    .AddInteractiveServerRenderMode();
            });

            return app;
        }
    }
}