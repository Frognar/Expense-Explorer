namespace ExpenseExplorer.API.Endpoints;

using System.Net;
using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using FunctionalCore.Failures;

public static class ReportEndpoints
{
  public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/reports");
    group.MapGet("/", GenerateReportAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GenerateReportAsync(
    DateOnly from,
    DateOnly to,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GenerateReportQuery query = new(from, to);
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
