namespace ExpenseExplorer.Application.Commands;

internal abstract class BaseCommandHandlerWrapper
{
  public abstract Task<object?> HandleAsync(
    object command,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken);
}
