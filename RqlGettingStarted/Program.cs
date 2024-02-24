namespace RqlGettingStarted
{
    using Rules.Framework;
    using Rules.Framework.Extension;
    using Rules.Framework.WebUI;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var appBuilder = WebApplication.CreateBuilder(args);

            appBuilder.Services.AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton);

            appBuilder.Services.AddTransient(sp =>
                RulesEngineBuilder.CreateRulesEngine()
                    .WithContentType<ContentTypes>() // Your content types.
                    .WithConditionType<ConditionTypes>() // Your condition types.
                    .SetInMemoryDataSource(sp) // Using the data source provider configured on dependency injection.
                    .Build());

            appBuilder.Services.AddRulesFrameworkUI();

            var app = appBuilder.Build();

            app.UseRouting();
            app.UseRulesFrameworkWebUI(sp => sp.GetService<RulesEngine<ContentTypes, ConditionTypes>>()!.CreateGenericEngine());

            app.Run();
        }
    }
}