namespace ExpenseExplorer.Application.Commands;

internal class Sender(
  IServiceProvider serviceProvider,
  IDictionary<Type, BaseCommandHandlerWrapper> commandHandlerWrappers)
  : ISender
{
  private readonly IDictionary<Type, BaseCommandHandlerWrapper> _commandHandlerWrappers = commandHandlerWrappers;

  public Task<TResponse> SendAsync<TResponse>(
    ICommand<TResponse> command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Type requestType = command.GetType();
    if (!_commandHandlerWrappers.TryGetValue(requestType, out BaseCommandHandlerWrapper? wrapper))
    {
      throw new InvalidOperationException($"No command handler wrapper found for command of type {requestType}.");
    }

    return ((CommandHandlerWrapper<TResponse>)wrapper).HandleAsync(command, serviceProvider, cancellationToken);
  }
}
