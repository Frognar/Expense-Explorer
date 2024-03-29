namespace ExpenseExplorer.Application.Commands;

using Microsoft.Extensions.DependencyInjection;

internal class CommandHandlerWrapperImpl<TCommand, TResponse> : CommandHandlerWrapper<TResponse>
  where TCommand : ICommand<TResponse>
{
  public override Task<TResponse> HandleAsync(
    ICommand<TResponse> command,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken)
  {
    return serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>()
      .HandleAsync((TCommand)command, cancellationToken);
  }
}
