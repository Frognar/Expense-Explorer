IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> db = builder
    .AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .AddDatabase("expense-explorer");

builder.AddProject<Projects.ExpenseExplorer_Api>("expense-explorer-api")
    .WithReference(db)
    .WaitFor(db)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
