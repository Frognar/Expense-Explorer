namespace ExpenseExplorer.API.Endpoints;

using System.Diagnostics;
using System.Net;
using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Queries;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using Receipt = ExpenseExplorer.Domain.Receipts.Receipt;

public static class ReceiptEndpoints
{
  private const string _getReceiptRoute = "GetReceipt";

  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapGet("/", GetReceiptsAsync);
    group.MapGet("/{receiptId}", GetReceiptAsync).WithName(_getReceiptRoute);
    group.MapPost("/", OpenNewReceiptAsync);
    group.MapPatch("/{receiptId}", UpdateReceiptAsync);
    group.MapPost("/{receiptId}/purchases", AddPurchaseAsync);
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
    Either<Failure, PageOf<ReceiptHeaders>> result = await sender.SendAsync(query, cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> GetReceiptAsync(
    string receiptId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetReceiptQuery query = new(receiptId);
    Either<Failure, ReadModel.Models.Receipt> result = await sender.SendAsync(query, cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
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
      .MapRight(r => r.MapTo<OpenNewReceiptResponse>())
      .Match(Handle, response => Results.CreatedAtRoute(_getReceiptRoute, new { receiptId = response.Id }, response));
  }

  private static async Task<IResult> UpdateReceiptAsync(
    string receiptId,
    UpdateReceiptRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    UpdateReceiptCommand command = new(receiptId, request.StoreName, request.PurchaseDate, today);
    Either<Failure, Receipt> result = await sender.SendAsync(command, cancellationToken);
    return result
      .MapRight(r => new UpdateReceiptResponse(r.Id.Value, r.Store.Name, r.PurchaseDate.Date, r.Version.Value))
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchaseAsync(
    string receiptId,
    AddPurchaseRequest request,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    Result<Receipt> result = await sender.SendAsync(request.MapToCommand(receiptId), cancellationToken);
    return result
      .Map(r => r.MapTo<AddPurchaseResponse>())
      .Match(Handle, response => Results.CreatedAtRoute(_getReceiptRoute, new { receiptId = response.Id }, response));
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
