using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Infrastructure;
using ExpenseExplorer.WebApp.Components;
using ExpenseExplorer.WebApp.Data;
using ExpenseExplorer.WebApp.Services;
using Radzen;
using IReceiptRepository = ExpenseExplorer.WebApp.Data.IReceiptRepository;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IReceiptRepository, InMemoryReceiptRepository>();
builder.Services
    .AddScoped<ExpenseExplorer.Application.Receipts.Data.IReceiptRepository>(_ => ReceiptRepositoryFactory.Create());

builder.Services.AddDatabase(
    builder.Configuration.GetConnectionString("expense-explorer") ?? throw new InvalidOperationException());

builder.Services.AddInfrastructure();
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