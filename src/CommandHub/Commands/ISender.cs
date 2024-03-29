namespace CommandHub.Commands;

public interface ISender
{
  Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
