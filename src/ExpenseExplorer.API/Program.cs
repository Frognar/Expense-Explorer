using CommandHub.DependencyInjection;
using ExpenseExplorer.API.Endpoints;
using ExpenseExplorer.API.Middlewares;
using ExpenseExplorer.Application;
using ExpenseExplorer.Infrastructure;
using ExpenseExplorer.ReadModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(
  new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
    .Build());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<TimeProvider>(_ => TimeProvider.System);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddReadModel(builder.Configuration);

builder.Services.AddCommandHub(
  typeof(ApplicationDependencyInjection).Assembly,
  typeof(ReadModelDependencyInjection).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.ConfigureReadModel();

app.MapEndpoints();

app.Run();

/// <summary>
/// For integration tests.
/// </summary>
#pragma warning disable S1118
public partial class Program;
#pragma warning restore S1118
