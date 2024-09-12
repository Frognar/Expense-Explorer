using DotResult;
using ExpenseExplorer.Domain.Facts;

namespace ExpenseExplorer.Application;

public interface IFactStore<T>
{
  public Task<Result<T>> ReadAsync(
    string streamId,
    Func<IEnumerable<Fact>, Result<T>> recreate,
    CancellationToken cancellationToken);

  public Task<Result<T>> SaveAsync(
    string streamId,
    T value,
    CancellationToken cancellationToken);
}
