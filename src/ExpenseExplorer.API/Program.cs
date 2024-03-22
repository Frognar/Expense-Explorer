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
