namespace ExpenseExplorer.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Infrastructure.Receipts.Projections.FactHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class FactProcessorJob(InMemoryMessageQueue queue, IServiceScopeFactory serviceScopeFactory)
  : BackgroundService
{
  private readonly InMemoryMessageQueue _queue = queue;
  private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (Fact fact in _queue.Reader.ReadAllAsync(stoppingToken))
    {
      using IServiceScope scope = _serviceScopeFactory.CreateScope();
      Task task = fact switch
      {
        ReceiptCreated receiptCreated => scope.ServiceProvider.GetRequiredService<ReceiptCreatedHandler>()
          .HandleAsync(receiptCreated, stoppingToken),
        _ => Task.FromResult(
          () => Console.WriteLine(System.Text.Encoding.UTF8.GetString(FactSerializer.Serialize(fact)))),
      };

      await task;
    }
  }
}
