namespace Rules.Framework.WebUI
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;

    /// <summary>
    /// IApplicationBuilder extension for Rules Framework Web UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="genericRulesEngineFactory">The generic rules engine factory.</param>
        /// <param name="webUIOptionsAction">The web UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app,
            Func<IServiceProvider, IGenericRulesEngine> genericRulesEngineFactory,
            Action<WebUIOptions> webUIOptionsAction)
        {
            var genericRulesEngine = genericRulesEngineFactory.Invoke(app.ApplicationServices);
            return app.UseRulesFrameworkWebUI(genericRulesEngine, webUIOptionsAction);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="genericRulesEngineFactory">The generic rules engine factory.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app,
            Func<IServiceProvider, IGenericRulesEngine> genericRulesEngineFactory)
        {
            return app.UseRulesFrameworkWebUI(genericRulesEngineFactory, null);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="genericRulesEngine">The generic rules engine.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app,
            IGenericRulesEngine genericRulesEngine)
        {
            return app.UseRulesFrameworkWebUI(genericRulesEngine, new WebUIOptions());
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="genericRulesEngine">The generic rules engine.</param>
        /// <param name="webUIOptionsAction">The web UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app,
            IGenericRulesEngine genericRulesEngine,
            Action<WebUIOptions> webUIOptionsAction)
        {
            WebUIOptions webUIOptions;

            using (var scope = app.ApplicationServices.CreateScope())
            {
                webUIOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<WebUIOptions>>().Value;
                webUIOptionsAction?.Invoke(webUIOptions);
            }

            return app.UseRulesFrameworkWebUI(genericRulesEngine, webUIOptions);
        }

        /// <summary>
        /// Uses the rules framework web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="genericRulesEngine">The generic rules engine.</param>
        /// <param name="webUIOptions">The web UI options.</param>
        /// <returns></returns>
        private static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app, IGenericRulesEngine genericRulesEngine,
            WebUIOptions webUIOptions)
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();

            app.UseMiddleware<WebUIMiddleware>(
                new List<IHttpRequestHandler>
                {
                    new GetIndexPageHandler(webUIOptions),
                    new GetConfigurationsHandler(genericRulesEngine, webUIOptions),
                    new GetContentTypeHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                    new GetRulesHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions)
                },
                webUIOptions);

            return app;
        }
    }
}