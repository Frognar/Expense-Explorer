using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

namespace ExpenseExplorer.API.Endpoints;

public static class CategoryEndpoints
{
  public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/categories");
    group.MapGet("/expenses", GetExpenseCategoriesAsync);
    group.MapGet("/incomes", GetIncomeCategoriesAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetExpenseCategoriesAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetExpenseCategoriesQuery query = new(pageSize, pageNumber, search);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> GetIncomeCategoriesAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetIncomeCategoriesQuery query = new(pageSize, pageNumber, search);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }
}
