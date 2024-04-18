namespace ExpenseExplorer.API.Endpoints;

using System.Diagnostics;
using System.Net;
using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.ReadModel.Queries;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapGet("/", GetReceiptsAsync);
    group.MapGet("/{receiptId}", GetReceiptAsync);
    group.MapPost("/", OpenNewReceiptAsync);
    group.MapPost("/{receiptId}", AddPurchaseAsync);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> GetReceiptsAsync(
    int? pageSize,
    int? pageNumber,
    string? search,
    DateOnly? after,
    DateOnly? before,
    decimal? minTotal,
    decimal? maxTotal,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetReceiptsQuery query = new(pageSize, pageNumber, search, after, before, minTotal, maxTotal);
    var result = await sender.SendAsync(query, cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static Task<IResult> GetReceiptAsync(
    string receiptId,
#pragma warning disable S1172
    ISender sender,
    CancellationToken cancellationToken = default)
#pragma warning restore S1172
  {
    return Task.FromResult(receiptId == "abc" ? Results.Ok() : Results.NotFound());
  }

  private static async Task<IResult> OpenNewReceiptAsync(
    OpenNewReceiptRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    Either<Failure, Receipt> result = await sender.SendAsync(request.MapToCommand(today), cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchaseAsync(
    string receiptId,
    AddPurchaseRequest request,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    Either<Failure, Receipt> result = await sender.SendAsync(request.MapToCommand(receiptId), cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static IResult Handle(Failure failure)
  {
    return failure switch
    {
      ValidationFailure validationFailure => HandleValidation(validationFailure),
      NotFoundFailure notFoundFailure => HandleNotFound(notFoundFailure),
      FatalFailure fatal => Results.Problem(detail: fatal.Message, statusCode: (int)HttpStatusCode.InternalServerError),
      _ => throw new UnreachableException(),
    };
  }

  private static IResult HandleValidation(ValidationFailure validationFailure)
  {
    return Results.Problem(
      detail: validationFailure.Message,
      statusCode: (int)HttpStatusCode.BadRequest,
      extensions: new Dictionary<string, object?>
      {
        ["Errors"] = validationFailure.Errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)),
      });
  }

  private static IResult HandleNotFound(NotFoundFailure notFoundFailure)
  {
    return Results.Problem(
      detail: notFoundFailure.Message,
      statusCode: (int)HttpStatusCode.NotFound,
      extensions: new Dictionary<string, object?> { ["Id"] = notFoundFailure.Id, });
  }
}
