using ExpenseExplorer.API.Endpoints;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<TimeProvider>(_ => TimeProvider.System);
string? connectionString = builder.Configuration.GetConnectionString("EventStore");
ArgumentNullException.ThrowIfNull(connectionString);
builder.Services.AddScoped<IReceiptRepository>(_ => new EventStoreReceiptRepository(connectionString));

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
