using CommandHub.Commands;

namespace CommandHub.Wrappers;

internal abstract class CommandHandlerWrapper<TResponse> : BaseHandlerWrapper
{
  public abstract Task<TResponse> HandleAsync(
    ICommand<TResponse> command,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken);
}
