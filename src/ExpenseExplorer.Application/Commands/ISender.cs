namespace ExpenseExplorer.Application.Commands;

public interface ISender
{
  Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
