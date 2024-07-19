namespace Rules.Framework.WebUI
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;

    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for Rules Framework Web UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="rulesEngineFactory">The rules engine factory.</param>
        /// <param name="webUIOptionsAction">The web UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            Func<IServiceProvider, IRulesEngine> rulesEngineFactory,
            Action<WebUIOptions> webUIOptionsAction)
        {
            var genericRulesEngine = rulesEngineFactory.Invoke(app.ApplicationServices);
            return app.UseRulesFrameworkWebUI(genericRulesEngine, webUIOptionsAction);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="rulesEngineFactory">The rules engine factory.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            Func<IServiceProvider, IRulesEngine> rulesEngineFactory)
        {
            return app.UseRulesFrameworkWebUI(rulesEngineFactory, null);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            IRulesEngine rulesEngine)
        {
            return app.UseRulesFrameworkWebUI(rulesEngine, new WebUIOptions());
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <param name="webUIOptionsAction">The web UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            IRulesEngine rulesEngine,
            Action<WebUIOptions> webUIOptionsAction)
        {
            WebUIOptions webUIOptions;

            using (var scope = app.ApplicationServices.CreateScope())
            {
                webUIOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<WebUIOptions>>().Value;
                webUIOptionsAction?.Invoke(webUIOptions);
            }

            return app.UseRulesFrameworkWebUI(rulesEngine, webUIOptions);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <param name="webUIOptions">The web UI options.</param>
        /// <returns></returns>
        private static IApplicationBuilder UseRulesFrameworkWebUI(
            this IApplicationBuilder app,
            IRulesEngine rulesEngine,
            WebUIOptions webUIOptions)
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();

            app.UseMiddleware<WebUIMiddleware>(
                new List<IHttpRequestHandler>
                {
                    new GetIndexPageHandler(webUIOptions),
                    new GetConfigurationsHandler(rulesEngine, webUIOptions),
                    new GetRulesetsHandler(rulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                    new GetRulesHandler(rulesEngine, ruleStatusDtoAnalyzer, webUIOptions)
                },
                webUIOptions);

            return app;
        }
    }
}