using Rules.Framework.Generic;
using Rules.Framework.WebUI;
using Rules.Framework.WebUI.Sample.Engine;
using Rules.Framework.WebUI.Sample.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var rulesProvider = new RulesEngineProvider(new RulesBuilder(new List<IContentTypes>()
            {
                new RulesRandomFactory()
            }));

builder.Services.AddSingleton(rulesProvider);

builder.Services.AddSingleton<IGenericRulesEngine>(d => rulesProvider
    .GetRulesEngineAsync()
    .ConfigureAwait(false)
    .GetAwaiter()
    .GetResult());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.UseAuthorization();

AddRulesFrameworkUI(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void AddRulesFrameworkUI(IApplicationBuilder app)
{
    var rulesEngine = app.ApplicationServices.GetRequiredService<IGenericRulesEngine>();

    app.UseRulesFrameworkUI(rulesEngine);
}