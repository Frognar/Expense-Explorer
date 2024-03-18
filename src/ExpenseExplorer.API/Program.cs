using ExpenseExplorer.API.Endpoints;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<TimeProvider>(_ => TimeProvider.System);
builder.Services.AddScoped<IReceiptRepository, InMemoryReceiptRepository>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
