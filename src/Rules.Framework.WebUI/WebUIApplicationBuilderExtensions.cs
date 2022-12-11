namespace Rules.Framework.WebUI
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;

    /// <summary>
    /// IApplicationBuilder extension for Rules Framework Web UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Register the Rules Framework Web UI.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="genericRulesEngine">The generic rules framework engine.</param>
        /// <returns>The application builder with rules framework Web UI middleware registered</returns>
        public static IApplicationBuilder UseRulesFrameworkWebUI(this IApplicationBuilder app, IGenericRulesEngine genericRulesEngine)
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();

            app.UseMiddleware<WebUIMiddleware>(
                new List<IHttpRequestHandler>
                {
                    new GetIndexPageHandler(new WebUIOptions()),
                    new GetPriorityCriteriasHandler(genericRulesEngine),
                    new GetContentTypeHandler(genericRulesEngine, ruleStatusDtoAnalyzer),
                    new GetRulesHandler(genericRulesEngine, ruleStatusDtoAnalyzer)
                });

            return app;
        }
    }
}