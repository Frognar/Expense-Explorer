using AppHost;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

IResourceBuilder<PostgresDatabaseResource> db = postgres
    .CreateDatabase("expense-explorer");

builder.AddProject<Projects.ExpenseExplorer_WebApp>("expense-explorer-web-app")
    .WithReference(db)
    .WaitFor(db);

await builder.Build().RunAsync();