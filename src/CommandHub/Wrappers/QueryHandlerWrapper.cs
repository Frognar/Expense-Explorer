using CommandHub.Queries;

namespace CommandHub.Wrappers;

internal abstract class QueryHandlerWrapper<TResponse> : BaseHandlerWrapper
{
  public abstract Task<TResponse> HandleAsync(
    IQuery<TResponse> query,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken);
}
