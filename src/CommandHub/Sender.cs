namespace CommandHub;

using CommandHub.Commands;
using CommandHub.Wrappers;

internal sealed class Sender(
  Func<Type, object> serviceProvider,
  IDictionary<Type, BaseHandlerWrapper> commandHandlerWrappers)
  : ISender
{
  private readonly IDictionary<Type, BaseHandlerWrapper> _commandHandlerWrappers = commandHandlerWrappers;
  private readonly Func<Type, object> _serviceProvider = serviceProvider;

  public Task<TResponse> SendAsync<TResponse>(
    ICommand<TResponse> command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Type requestType = command.GetType();
    if (!_commandHandlerWrappers.TryGetValue(requestType, out BaseHandlerWrapper? wrapper))
    {
      throw new InvalidOperationException($"No command handler wrapper found for command of type {requestType}.");
    }

    return ((CommandHandlerWrapper<TResponse>)wrapper)
      .HandleAsync(command, _serviceProvider, cancellationToken);
  }
}