namespace ExpenseExplorer.API.Endpoints;

using System.Net;
using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using FunctionalCore.Failures;

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

  private static IResult Handle(Failure failure)
  {
    return failure.Match(HandleFatal, HandleNotFound, HandleValidation);
  }

  private static IResult HandleFatal(string message, Exception ex)
  {
    return Results.Problem(detail: message, statusCode: (int)HttpStatusCode.InternalServerError);
  }

  private static IResult HandleValidation(string message, IEnumerable<ValidationError> errors)
  {
    return Results.Problem(
      detail: message,
      statusCode: (int)HttpStatusCode.BadRequest,
      extensions: new Dictionary<string, object?>
      {
        ["Errors"] = errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)),
      });
  }

  private static IResult HandleNotFound(string message, string id)
  {
    return Results.Problem(
      detail: message,
      statusCode: (int)HttpStatusCode.NotFound,
      extensions: new Dictionary<string, object?> { ["Id"] = id, });
  }
}
