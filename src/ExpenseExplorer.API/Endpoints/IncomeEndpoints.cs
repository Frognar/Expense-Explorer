namespace ExpenseExplorer.API.Endpoints;

using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Incomes.Commands;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

public static class IncomeEndpoints
{
  private const string _getIncomeRoute = "GetIncome";

  public static IEndpointRouteBuilder MapIncomeEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/incomes");
    group.MapGet("/", GetIncomesAsync);
    group.MapGet("/{incomeId}", GetIncomeAsync).WithName(_getIncomeRoute);
    group.MapPost("/", AddIncomeAsync);
    group.MapPatch("/{incomeId}", UpdateIncomeDetailsAsync);
    group.MapDelete("/{incomeId}", DeleteIncomeAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetIncomesAsync(
    int? pageSize,
    int? pageNumber,
    string? source,
    string? category,
    string? description,
    DateOnly? receivedAfter,
    DateOnly? receivedBefore,
    decimal? minAmount,
    decimal? maxAmount,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetIncomesQuery query = new(
      pageSize,
      pageNumber,
      source,
      category,
      description,
      receivedAfter,
      receivedBefore,
      minAmount,
      maxAmount);

    return (await sender.SendAsync(query, cancellationToken))
      .Map(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> GetIncomeAsync(
    string incomeId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetIncomeQuery query = new(incomeId);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
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
      .Match(Handle, response => Results.CreatedAtRoute(_getIncomeRoute, new { incomeId = response.Id }, response));
  }

  private static async Task<IResult> UpdateIncomeDetailsAsync(
    string incomeId,
    UpdateIncomeDetailsRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken)
  {
    var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    return (await sender.SendAsync(request.MapToCommand(incomeId, today), cancellationToken))
      .Map(r => r.MapTo<UpdateIncomeDetailsResponse>())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> DeleteIncomeAsync(
    string incomeId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    return (await sender.SendAsync(new DeleteIncomeCommand(incomeId), cancellationToken))
      .Match(Handle, _ => Results.NoContent());
  }
}
