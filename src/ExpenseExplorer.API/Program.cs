using ExpenseExplorer.API.Endpoints;
using ExpenseExplorer.Application;
using ExpenseExplorer.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(
  new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<TimeProvider>(_ => TimeProvider.System);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

/// <summary>
/// For integration tests.
/// </summary>
#pragma warning disable S1118
public partial class Program;
#pragma warning restore S1118
