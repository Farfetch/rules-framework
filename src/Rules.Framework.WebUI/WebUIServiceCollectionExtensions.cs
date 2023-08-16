namespace Rules.Framework.WebUI
{
    using Microsoft.Extensions.DependencyInjection;
    using RazorLight;

    public static class WebUIServiceCollectionExtensions
    {
        public static IServiceCollection AddRulesFrameworkUI(this IServiceCollection services)
        {
            services.AddSingleton<IRazorLightEngine>(sp => new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(WebUIOptions))
                .SetOperatingAssembly(typeof(WebUIOptions).Assembly)
                .UseMemoryCachingProvider()
                .Build());

            return services;
        }
    }
}