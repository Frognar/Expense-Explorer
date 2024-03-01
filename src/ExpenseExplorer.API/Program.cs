WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/hello-world", () => "Hello World!");

app.Run();
