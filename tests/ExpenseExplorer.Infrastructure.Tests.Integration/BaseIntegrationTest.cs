namespace ExpenseExplorer.Infrastructure.Tests.Integration;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Testcontainers.EventStoreDb;

public class BaseIntegrationTest : IAsyncLifetime
{
  private readonly EventStoreDbContainer _container = new EventStoreDbBuilder()
    .WithImage("eventstore/eventstore:24.2.0-jammy")
    .Build();

  protected IReceiptRepository ReceiptRepository { get; private set; } = default!;

  public async Task InitializeAsync()
  {
    await _container.StartAsync();
    ReceiptRepository = new EventStoreReceiptRepository(_container.GetConnectionString());
  }

  public async Task DisposeAsync()
  {
    await _container.StopAsync();
    await _container.DisposeAsync();
  }
}
