using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.EventStoreDb;
using Testcontainers.PostgreSql;

namespace ExpenseExplorer.API.Tests.Integration;

public class ReceiptApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly EventStoreDbContainer _eventStoreDbContainer = new EventStoreDbBuilder()
    .WithImage("eventstore/eventstore:24.2.0-jammy")
    .Build();

  private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
    .WithImage("postgres")
    .Build();

  public async Task InitializeAsync()
  {
    await _eventStoreDbContainer.StartAsync();
    await _postgreSqlContainer.StartAsync();
    using var scope = Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseExplorerContext>();
    await dbContext.Database.MigrateAsync();
  }

  public new async Task DisposeAsync()
  {
    await _eventStoreDbContainer.StopAsync();
    await _eventStoreDbContainer.DisposeAsync();
    await _postgreSqlContainer.StopAsync();
    await _postgreSqlContainer.DisposeAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(
      sc =>
      {
        sc.RemoveAll(typeof(IReceiptRepository));
        sc.AddScoped<IReceiptRepository>(
          _ => new EventStoreReceiptRepository(_eventStoreDbContainer.GetConnectionString()));

        ServiceDescriptor serviceDescriptor = sc.Single(
          d => d.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, FactProcessor>));

        sc.Remove(serviceDescriptor);

        sc.RemoveAll(typeof(ExpenseExplorerContext));
        sc.AddScoped(_ => new ExpenseExplorerContext(_postgreSqlContainer.GetConnectionString()));
      });
  }
}
