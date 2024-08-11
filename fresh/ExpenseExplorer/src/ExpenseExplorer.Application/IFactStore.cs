namespace ExpenseExplorer.Application;

public interface IFactStore<T>
{
  public Task<T> ReadAsync(string streamId);

  public Task<T> SaveAsync(string streamId, T value);
}
