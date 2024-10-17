namespace Rules.Framework.WebUI.Sample
{
    using global::Rules.Framework.WebUI.Sample.Engine;
    using global::Rules.Framework.WebUI.Sample.ReadmeExample;
    using global::Rules.Framework.WebUI.Sample.Rules;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRulesFrameworkWebUI(registrar =>
                {
                    registrar.AddInstance("Readme example", (_, _) => new BasicRulesEngineExample().RulesEngine)
                        .AddInstance("Random rules example", async (_, _) =>
                        {
                            var rulesProvider = new RulesEngineProvider(new RulesBuilder(new List<IRuleSpecificationsProvider>()
                            {
                                new RulesRandomFactory()
                            }));

                            return await rulesProvider.GetRulesEngineAsync();
                        });
                });

            builder.Logging.SetMinimumLevel(LogLevel.Trace).AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAntiforgery();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseRulesFrameworkWebUI();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}