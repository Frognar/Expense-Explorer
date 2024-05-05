namespace ExpenseExplorer.API.Endpoints;

using System.Net;
using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
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
    group.MapPatch("/{receiptId}/purchases/{purchaseId}", UpdatePurchaseAsync);
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
    Result<Receipt> result = await sender.SendAsync(request.MapToCommand(today), cancellationToken);
    return result
      .Map(r => r.MapTo<OpenNewReceiptResponse>())
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
    Result<Receipt> result = await sender.SendAsync(request.MapToCommand(receiptId, today), cancellationToken);
    return result
      .Map(r => r.MapTo<UpdateReceiptResponse>())
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

  private static async Task<IResult> UpdatePurchaseAsync(
    string receiptId,
    string purchaseId,
    UpdatePurchaseRequest request,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    Result<Receipt> result = await sender.SendAsync(request.MapToCommand(receiptId, purchaseId), cancellationToken);
    return result
      .Map(r => r.MapTo<UpdatePurchaseResponse>())
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
