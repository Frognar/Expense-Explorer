namespace ExpenseExplorer.API.Tests.Integration;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using ExpenseExplorer.ReadModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.EventStoreDb;
using Testcontainers.PostgreSql;

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

        sc.RemoveAll(typeof(ExpenseExplorerContext));
        sc.AddScoped(_ => new ExpenseExplorerContext(_postgreSqlContainer.GetConnectionString()));
      });
  }
}
