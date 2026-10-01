IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> db = builder
    .AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .AddDatabase("expense-explorer");

IResourceBuilder<ContainerResource> ocr = builder
    .AddDockerfile("ocr", "../../../ocr")
    .WithHttpEndpoint(targetPort: 8000);

builder.AddProject<Projects.ExpenseExplorer_Api>("expense-explorer-api")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("Ocr__Url", ocr.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
