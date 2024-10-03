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
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            AddRulesFrameworkUI(app, useReadmeExample: false);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void AddRulesFrameworkUI(IApplicationBuilder app, bool useReadmeExample = false)
        {
            if (useReadmeExample)
            {
                app.UseRulesFrameworkWebUI(new BasicRulesEngineExample().RulesEngine);

                return;
            }

            var rulesProvider = new RulesEngineProvider(new RulesBuilder(new List<IRuleSpecificationsProvider>()
            {
                new RulesRandomFactory()
            }));

            var rulesEngine = rulesProvider
                .GetRulesEngineAsync()
                .GetAwaiter()
                .GetResult();

            app.UseRulesFrameworkWebUI(rulesEngine);
        }
    }
}