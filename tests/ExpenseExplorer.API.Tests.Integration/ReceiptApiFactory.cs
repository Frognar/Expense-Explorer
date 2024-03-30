namespace ExpenseExplorer.API.Tests.Integration;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.EventStoreDb;

public class ReceiptApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly EventStoreDbContainer _container = new EventStoreDbBuilder()
    .WithImage("eventstore/eventstore:24.2.0-jammy")
    .Build();

  public Task InitializeAsync()
  {
    return _container.StartAsync();
  }

  public new async Task DisposeAsync()
  {
    await _container.StopAsync();
    await _container.DisposeAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(
      sc =>
      {
        sc.RemoveAll(typeof(IReceiptRepository));
        sc.AddScoped<IReceiptRepository>(_ => new EventStoreReceiptRepository(_container.GetConnectionString()));
      });
  }
}
