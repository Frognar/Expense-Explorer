namespace ExpenseExplorer.API.Endpoints;

using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

public static class CategoryEndpoints
{
  public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/categories");
    group.MapGet("/expenses", GetCategoriesAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetCategoriesAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetCategoriesQuery query = new(pageSize, pageNumber, search);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }
}
