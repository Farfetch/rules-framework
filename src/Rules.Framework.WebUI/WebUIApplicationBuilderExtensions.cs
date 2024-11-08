namespace Rules.Framework.WebUI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Components;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Rules.Framework.WebUI.Services;

    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for Rules Framework Web UI
    /// </summary>
    [ExcludeFromCodeCoverage]
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
        /// <param name="webUIOptionsAction">The web UI options configuration action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            Action<WebUIOptions> webUIOptionsAction)
        {
            var rulesEngineInstanceProvider = app.ApplicationServices.GetRequiredService<RulesEngineInstanceProvider>();
            rulesEngineInstanceProvider.EnumerateInstances(app.ApplicationServices);

            // Options
            var webUIOptions = new WebUIOptions();
            webUIOptionsAction.Invoke(webUIOptions);
            var webUIOptionsRegistry = app.ApplicationServices.GetRequiredService<WebUIOptionsRegistry>();
            webUIOptionsRegistry.Register(webUIOptions);

            // Blazor
            var embeddedProvider = new EmbeddedFileProvider(typeof(WebUIApplicationBuilderExtensions).Assembly, "Rules.Framework.WebUI.Assets");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = embeddedProvider,
                RequestPath = new PathString("/rules-ui")
            });

            app.UseEndpoints(builder =>
            {
                builder.MapGet("/rules-ui", ctx =>
                {
                    ctx.Response.Redirect("/rules-ui/instance");
                    return Task.CompletedTask;
                });

                builder.MapRazorComponents<WebUIApp>()
                    .AddInteractiveServerRenderMode();
            });

            return app;
        }
    }
}