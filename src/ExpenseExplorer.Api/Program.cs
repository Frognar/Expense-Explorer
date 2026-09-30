using ExpenseExplorer.Api;
using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.Services.AddProblemDetails();
builder.Services.Configure<ExceptionHandlerOptions>(options =>
    options.StatusCodeSelector = exception => exception is BadHttpRequestException badRequest
        ? badRequest.StatusCode
        : StatusCodes.Status500InternalServerError);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(TimeProvider.System);
builder.AddAuth();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString(InfrastructureSetup.ConnectionStringName)
    ?? throw new InvalidOperationException($"Connection string '{InfrastructureSetup.ConnectionStringName}' is missing."));

WebApplication app = builder.Build();
await app.Services.MigrateDatabaseAsync();

if (UserCommandLine.IsInvoked(args))
{
    return await UserCommandLine.RunAsync(
        app.Services, args, UserCommandLine.ReadPasswordFromConsole, Console.Out, CancellationToken.None);
}

app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = (log, http) =>
    log.Set("UserName", http.User.Identity?.Name ?? "anonymous"));
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapHealthChecks("/health");
app.MapApi();
app.MapFrontendFallback();

await app.RunAsync();
return 0;
