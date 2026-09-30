using ExpenseExplorer.Api;
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
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString(InfrastructureSetup.ConnectionStringName)
    ?? throw new InvalidOperationException($"Connection string '{InfrastructureSetup.ConnectionStringName}' is missing."));

WebApplication app = builder.Build();
await app.Services.MigrateDatabaseAsync();

app.UseSerilogRequestLogging();
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

app.MapHealthChecks("/health");
app.MapApi();
app.MapFrontendFallback();

await app.RunAsync();
