using CommandHub;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

namespace ExpenseExplorer.API.Endpoints;

public static class ReportEndpoints
{
  public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/reports");
    group.MapGet("/category-based-expense", GenerateCategoryBasedExpenseReportAsync);
    group.MapGet("/income-to-expense", GenerateIncomeToExpenseReportAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GenerateCategoryBasedExpenseReportAsync(
    DateOnly from,
    DateOnly to,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GenerateCategoryBasedExpenseReportQuery query = new(from, to);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> GenerateIncomeToExpenseReportAsync(
    DateOnly from,
    DateOnly to,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GenerateIncomeToExpenseReportQuery query = new(from, to);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(page => page.MapToResponse())
      .Match(Handle, Results.Ok);
  }
}
