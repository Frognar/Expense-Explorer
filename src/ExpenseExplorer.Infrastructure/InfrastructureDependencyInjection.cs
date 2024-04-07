namespace ExpenseExplorer.Infrastructure;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureDependencyInjection
{
  public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddEventStore(configuration);
  }

  private static void AddEventStore(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("EventStore");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddScoped<IReceiptRepository>(_ => new EventStoreReceiptRepository(connectionString));
  }
}
