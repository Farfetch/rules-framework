namespace Rules.Framework.WebUI
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework.WebUI.Services;

    public static class WebUIMvcBuilderExtensions
    {
        public static IMvcBuilder AddRulesFrameworkWebUI(this IMvcBuilder mvcBuilder, Action<IRulesEngineInstancesRegistrar> instancesRegistrationAction)
        {
            var rulesEngineInstanceProvider = new RulesEngineInstanceProvider();
            instancesRegistrationAction.Invoke(rulesEngineInstanceProvider);
            mvcBuilder.Services.AddSingleton(rulesEngineInstanceProvider);
            mvcBuilder.Services.AddSingleton<IRulesEngineInstanceProvider>(rulesEngineInstanceProvider);

            // Blazor
            mvcBuilder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            mvcBuilder.Services.AddBlazorBootstrap();

            return mvcBuilder
                .AddApplicationPart(typeof(WebUIMvcBuilderExtensions).Assembly);
        }
    }
}