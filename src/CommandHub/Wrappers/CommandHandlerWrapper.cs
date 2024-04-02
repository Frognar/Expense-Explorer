namespace CommandHub.Wrappers;

using CommandHub.Commands;

internal abstract class CommandHandlerWrapper<TResponse> : BaseCommandHandlerWrapper
{
  public abstract Task<TResponse> HandleAsync(
    ICommand<TResponse> command,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken);
}
