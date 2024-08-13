using System.Diagnostics.CodeAnalysis;
using CommandHub.Queries;

namespace CommandHub.Wrappers;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated via reflection.")]
internal sealed class QueryHandlerWrapperImpl<TQuery, TResponse> : QueryHandlerWrapper<TResponse>
  where TQuery : IQuery<TResponse>
{
  private readonly Type _handlerType = typeof(IQueryHandler<TQuery, TResponse>);

  public override Task<TResponse> HandleAsync(
    IQuery<TResponse> query,
    Func<Type, object> serviceProvider,
    CancellationToken cancellationToken)
  {
    var handler = (IQueryHandler<TQuery, TResponse>)serviceProvider(_handlerType);
    return handler.HandleAsync((TQuery)query, cancellationToken);
  }
}
