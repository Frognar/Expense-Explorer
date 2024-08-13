using DotResult;

namespace ExpenseExplorer.Application;

public interface IFactStore<T>
{
  public Task<Result<T>> ReadAsync(
    string streamId,
    CancellationToken cancellationToken);

  public Task<Result<T>> SaveAsync(
    T value,
    CancellationToken cancellationToken);
}