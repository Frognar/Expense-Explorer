using ExpenseExplorer.Application;
using ExpenseExplorer.Infrastructure;
using ExpenseExplorer.WebApp.Components;
using ExpenseExplorer.WebApp.Services;
using Radzen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDatabase(
    builder.Configuration.GetConnectionString("expense-explorer") ?? throw new InvalidOperationException());

builder.Services.AddInfrastructure()
    .AddApplication();

builder.Services.AddScoped<ReceiptService>();

builder.Services.AddRadzenComponents();
WebApplication app = builder.Build();
string[] supportedCultures = ["pl-PL", "en-US"];
RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.Services.InitializeDatabase();

await app.RunAsync();