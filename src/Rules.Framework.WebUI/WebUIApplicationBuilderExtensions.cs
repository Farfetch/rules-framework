namespace Rules.Framework.WebUI
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Rules.Framework.Generic;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;

    /// <summary>
    /// IApplicationBuilder extension for Rules Framework UI
    /// </summary>
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Register the UI middleware
        /// </summary>
        public static IApplicationBuilder UseRulesFrameworkUI(this IApplicationBuilder app, IGenericRulesEngine genericRulesEngine)
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