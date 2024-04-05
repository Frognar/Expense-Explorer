namespace ExpenseExplorer.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Application;
using ExpenseExplorer.Domain.Receipts.Facts;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class FactBus(InMemoryMessageQueue queue) : IFactBus
{
  public async Task PublishAsync<T>(T fact, CancellationToken cancellationToken = default)
    where T : Fact
  {
    await queue.Writer.WriteAsync(fact, cancellationToken);
  }
}
