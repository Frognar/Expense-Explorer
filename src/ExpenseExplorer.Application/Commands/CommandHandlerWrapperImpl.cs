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
    return Handler();

    Task<TResponse> Handler()
      => serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>()
        .HandleAsync((TCommand)command, cancellationToken);
  }

  public override async Task<object?> HandleAsync(
    object command,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken)
  {
    return await HandleAsync((ICommand<TResponse>)command, serviceProvider, cancellationToken);
  }
}
