using ExpenseExplorer.API.Endpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<TimeProvider>(_ => TimeProvider.System);

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
