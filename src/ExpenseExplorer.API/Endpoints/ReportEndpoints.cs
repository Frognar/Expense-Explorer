namespace ExpenseExplorer.API.Endpoints;

using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

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
}
