namespace ExpenseExplorer.Application;

using ExpenseExplorer.Domain.Receipts.Facts;

public interface IFactBus
{
  Task PublishAsync<T>(T fact, CancellationToken cancellationToken = default)
    where T : Fact;
}
