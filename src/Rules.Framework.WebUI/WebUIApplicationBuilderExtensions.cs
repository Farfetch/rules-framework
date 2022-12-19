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
        /// <param name="genericRulesEngine">The generic rules engine.</param>
        /// <param name="uiOptionsAction">The UI options action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app, IGenericRulesEngine genericRulesEngine,
            Action<WebUIOptions> uiOptionsAction = null)
        {
            WebUIOptions webUIOptions;

            using (var scope = app.ApplicationServices.CreateScope())
            {
                webUIOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<WebUIOptions>>().Value;
                uiOptionsAction?.Invoke(webUIOptions);
            }

            return UseRulesFrameworkWebUI(app, genericRulesEngine, webUIOptions);
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
                    new GetPriorityCriteriaHandler(genericRulesEngine, webUIOptions),
                    new GetContentTypeHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                    new GetRulesHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions)
                },
                webUIOptions);

            return app;
        }
    }
}