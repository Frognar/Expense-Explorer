namespace ExpenseExplorer.Infrastructure;

using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureDependencyInjection
{
  public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    string? connectionString = configuration.GetConnectionString("EventStore");
    ArgumentNullException.ThrowIfNull(connectionString);
    serviceCollection.AddSingleton<InMemoryMessageQueue>();
    serviceCollection.AddSingleton<IFactBus, FactBus>();
    serviceCollection.AddScoped<IReceiptRepository>(_ => new EventStoreReceiptRepository(connectionString));
    serviceCollection.AddHostedService<FactProcessorJob>();
  }
}
