namespace ExpenseExplorer.API.Endpoints;

using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

public static class IncomeEndpoints
{
  public static IEndpointRouteBuilder MapIncomeEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/incomes");
    group.MapPost("/", AddIncomeAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> AddIncomeAsync(
    AddIncomeRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    return (await sender.SendAsync(request.MapToCommand(today), cancellationToken))
      .Map(r => r.MapTo<AddIncomeResponse>())
      .Match(Handle, response => Results.Created((string?)null, response));
  }
}
