namespace ExpenseExplorer.Application.Commands;

internal abstract class CommandHandlerWrapper<TResponse> : BaseCommandHandlerWrapper
{
  public abstract Task<TResponse> HandleAsync(
    ICommand<TResponse> command,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken);
}
