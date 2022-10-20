using Rules.Framework;
using Rules.Framework.Admin.UI.Sample.Enums;
using Rules.Framework.Admin.UI.Sample.Rules;
using Rules.Framework.UI;
using Rules.Framework.UI.Sample;
using Rules.Framework.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<RulesEngineProvider>(
    new RulesEngineProvider(new RulesBuilder(new List<IContentTypes>()
            {
                new RulesRandomFactory()
            }))
    );

builder.Services.AddSingleton<RulesEngine<ContentTypes, ConditionTypes>>(d =>
    d.GetRequiredService<RulesEngineProvider>().GetRulesEngineAsync()
    .GetAwaiter()
    .GetResult());

builder.Services.AddSingleton<IRulesService>(d => new RulesService(
    d.GetRequiredService<RulesEngineProvider>()));

var d = builder.Services.FirstOrDefault(d =>
d.ServiceType.IsGenericType &&
d.ServiceType.GetGenericTypeDefinition() == typeof(RulesEngine<,>));

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

app.UseRulesFrameworkUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();