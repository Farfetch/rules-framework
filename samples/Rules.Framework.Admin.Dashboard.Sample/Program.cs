using Rules.Framework.Admin.Dashboard.Sample.Engine;
using Rules.Framework.Admin.Dashboard.Sample.Rules;
using Rules.Framework.Admin.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(d => new RulesEngineProvider(new RulesBuilder(new List<IContentTypes>()
            {
                new TestNumberRules()
            })));

builder.Services.AddSingleton(d => d
.GetRequiredService<RulesEngineProvider>()
.GetRulesEngineAsync()
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

app.UseRulesFrameworkUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();