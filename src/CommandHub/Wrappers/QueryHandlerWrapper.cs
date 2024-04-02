namespace CommandHub.Wrappers;

using CommandHub.Queries;

internal abstract class QueryHandlerWrapper<TResponse> : BaseHandlerWrapper
{
  public abstract Task<TResponse> HandleAsync(
    IQuery<TResponse> query,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken);
}
