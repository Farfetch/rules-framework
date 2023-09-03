namespace Rules.Framework.WebUI
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using RazorLight;
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

            app.UseEndpoints(builder =>
            {
                var razorLightEngine = builder.ServiceProvider.GetRequiredService<IRazorLightEngine>();
                var httpRequestHandlers = new List<IHttpRequestHandler>
                {
                    new GetIndexPageHandler(razorLightEngine, webUIOptions),
                    new GetConfigurationsHandler(genericRulesEngine, webUIOptions),
                    new GetContentTypeHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                    new GetRulesHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                    new PostRqlHandler(genericRulesEngine, ruleStatusDtoAnalyzer, webUIOptions),
                };

                foreach (var httpRequestHandler in httpRequestHandlers)
                {
                    foreach (var routePattern in httpRequestHandler.ResourcePaths)
                    {
                        switch (httpRequestHandler.HttpMethod)
                        {
                            case HttpMethod.GET:
                                builder.MapGet(routePattern, async (httpContext) => await httpRequestHandler.HandleAsync(httpContext).ConfigureAwait(false));
                                break;

                            case HttpMethod.POST:
                                builder.MapPost(routePattern, async (httpContext) => await httpRequestHandler.HandleAsync(httpContext).ConfigureAwait(false));
                                break;

                            case HttpMethod.PUT:
                                builder.MapPut(routePattern, async (httpContext) => await httpRequestHandler.HandleAsync(httpContext).ConfigureAwait(false));
                                break;

                            case HttpMethod.DELETE:
                                builder.MapDelete(routePattern, async (httpContext) => await httpRequestHandler.HandleAsync(httpContext).ConfigureAwait(false));
                                break;

                            default:
                                throw new NotSupportedException($"Mapping of specified http method is not supported: {httpRequestHandler.HttpMethod}.");
                        }
                    }
                }
            });
            app.UseMiddleware<WebUIMiddleware>(webUIOptions);

            return app;
        }
    }
}