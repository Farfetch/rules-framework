using Rules.Framework.Admin.UI.Sample.Rules;
using Rules.Framework.UI;
using Rules.Framework.UI.Sample;
using Rules.Framework.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(
    new RulesEngineProvider(new RulesBuilder(new List<IContentTypes>()
            {
                new RulesRandomFactory()
            }))
    );

builder.Services.AddSingleton<IRulesService>(d => new RulesService(
    d.GetRequiredService<RulesEngineProvider>()));

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