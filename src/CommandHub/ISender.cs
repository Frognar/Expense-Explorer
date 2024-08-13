using CommandHub.Commands;
using CommandHub.Queries;

namespace CommandHub;

public interface ISender
{
  Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

  Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
