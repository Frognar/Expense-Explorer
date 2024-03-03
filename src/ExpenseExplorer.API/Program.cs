using ExpenseExplorer.API.Contract;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/hello-world", () => "Hello World!");
app.MapPost("/api/receipts", (OpenNewReceiptRequest r) => Results.Ok(r));

app.Run();
