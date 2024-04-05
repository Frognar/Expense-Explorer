namespace ExpenseExplorer.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using Microsoft.Extensions.Hosting;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class FactProcessorJob(InMemoryMessageQueue queue) : BackgroundService
{
  private readonly InMemoryMessageQueue _queue = queue;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (Fact fact in _queue.Reader.ReadAllAsync(stoppingToken))
    {
      Console.WriteLine(System.Text.Encoding.UTF8.GetString(FactSerializer.Serialize(fact)));
    }
  }
}
