using System.Globalization;
using ExpenseExplorer.Web;
using ExpenseExplorer.Web.Api;
using ExpenseExplorer.Web.Auth;
using ExpenseExplorer.Web.Localization;
using ExpenseExplorer.Web.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Uri baseAddress = new(builder.HostEnvironment.BaseAddress);
builder.Services.AddSingleton(TimeProvider.System);

// Auth calls go out without a token; everything else goes through the handler that adds one.
builder.Services.AddScoped(services => new AuthSession(
    new HttpClient { BaseAddress = baseAddress },
    services.GetRequiredService<TimeProvider>()));
builder.Services.AddScoped(services => new ExpenseApi(new HttpClient(new BearerTokenHandler(
    services.GetRequiredService<AuthSession>(),
    services.GetRequiredService<NavigationManager>())
{
    InnerHandler = new HttpClientHandler(),
})
{
    BaseAddress = baseAddress,
}));
builder.Services.AddScoped<Language>();
builder.Services.AddScoped<Appearance>();
builder.Services.AddMudServices();

WebAssemblyHost host = builder.Build();

// The language is chosen before the first render, so dates and numbers are formatted to match.
Language language = host.Services.GetRequiredService<Language>();
await language.LoadAsync(host.Services.GetRequiredService<IJSRuntime>());
CultureInfo.DefaultThreadCurrentCulture = language.Culture;
CultureInfo.DefaultThreadCurrentUICulture = language.Culture;
await host.Services.GetRequiredService<Appearance>().LoadAsync(host.Services.GetRequiredService<IJSRuntime>());

await host.RunAsync();
