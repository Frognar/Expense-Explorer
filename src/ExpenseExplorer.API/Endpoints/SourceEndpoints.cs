using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

namespace ExpenseExplorer.API.Endpoints;

public static class SourceEndpoints
{
  public static IEndpointRouteBuilder MapSourceEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/sources");
    group.MapGet("/", GetSourcesAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetSourcesAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetSourcesQuery query = new(pageSize, pageNumber, search);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }
}
