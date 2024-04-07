namespace ExpenseExplorer.ReadModel;

using CommandHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ReadModelDependencyInjection
{
  public static void AddReadModel(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddReadModelDb(configuration);
    serviceCollection.AddFactProcessor(configuration);
  }

  private static void AddFactProcessor(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("EventStore");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddHostedService(sp => new FactProcessor(connectionString, sp.GetRequiredService<ISender>()));
  }

  private static void AddReadModelDb(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("Postgres");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddScoped<ExpenseExplorerContext>(_ => new ExpenseExplorerContext(connectionString));
  }
}
