using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

namespace ExpenseExplorer.API.Endpoints;

public static class StoreEndpoints
{
  public static IEndpointRouteBuilder MapStoreEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/stores");
    group.MapGet("/", GetStoresAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetStoresAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetStoresQuery query = new(pageSize, pageNumber, search);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }
}
