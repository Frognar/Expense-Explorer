namespace ExpenseExplorer.Infrastructure;

using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Projections;
using ExpenseExplorer.Infrastructure.Receipts.Projections.FactHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureDependencyInjection
{
  public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddEventStore(configuration);
    serviceCollection.AddProjectionDb(configuration);
    serviceCollection.AddSingleton<InMemoryMessageQueue>();
    serviceCollection.AddSingleton<IFactBus, FactBus>();
    serviceCollection.AddHostedService<FactProcessorJob>();
  }

  private static void AddEventStore(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("EventStore");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddScoped<IReceiptRepository>(_ => new EventStoreReceiptRepository(connectionString));
  }

  private static void AddProjectionDb(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("Postgres");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddScoped<ExpenseExplorerContext>(_ => new ExpenseExplorerContext(connectionString));

    serviceCollection.AddProjectionHandlers();
  }

  private static void AddProjectionHandlers(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddTransient<ReceiptCreatedHandler>();
  }
}
