using CommandHub.Commands;
using CommandHub.Queries;
using CommandHub.Wrappers;

namespace CommandHub;

internal sealed class Sender(
  Func<Type, object> serviceProvider,
  IDictionary<Type, BaseHandlerWrapper> handlerWrappers)
  : ISender
{
  private readonly IDictionary<Type, BaseHandlerWrapper> _handlerWrappers = handlerWrappers;
  private readonly Func<Type, object> _serviceProvider = serviceProvider;

  public Task<TResponse> SendAsync<TResponse>(
    ICommand<TResponse> command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Type commandType = command.GetType();
    if (!_handlerWrappers.TryGetValue(commandType, out BaseHandlerWrapper? wrapper))
    {
      throw new InvalidOperationException($"No command handler wrapper found for command of type {commandType}.");
    }

    return ((CommandHandlerWrapper<TResponse>)wrapper)
      .HandleAsync(command, _serviceProvider, cancellationToken);
  }

  public Task<TResponse> SendAsync<TResponse>(
    IQuery<TResponse> query,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(query);
    Type queryType = query.GetType();
    if (!_handlerWrappers.TryGetValue(queryType, out BaseHandlerWrapper? wrapper))
    {
      throw new InvalidOperationException($"No query handler wrapper found for query of type {queryType}.");
    }

    return ((QueryHandlerWrapper<TResponse>)wrapper)
      .HandleAsync(query, _serviceProvider, cancellationToken);
  }
}
