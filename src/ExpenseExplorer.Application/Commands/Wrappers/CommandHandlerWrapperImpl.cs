namespace ExpenseExplorer.Application.Commands.Wrappers;

internal class CommandHandlerWrapperImpl<TCommand, TResponse> : CommandHandlerWrapper<TResponse>
  where TCommand : ICommand<TResponse>
{
  private readonly Type _handlerType = typeof(ICommandHandler<TCommand, TResponse>);

  public override Task<TResponse> HandleAsync(
    ICommand<TResponse> command,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken)
  {
    var handler = (ICommandHandler<TCommand, TResponse>)serviceProvider(_handlerType);
    return handler.HandleAsync((TCommand)command, cancellationToken);
  }
}
