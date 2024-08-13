using CommandHub;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.ReadModel.Queries;
using static ExpenseExplorer.API.Endpoints.FailureHandler;

namespace ExpenseExplorer.API.Endpoints;

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
    group.MapDelete("/{receiptId}", DeleteReceiptAsync);

    group.MapPost("/{receiptId}/purchases", AddPurchaseAsync);
    group.MapPatch("/{receiptId}/purchases/{purchaseId}", UpdatePurchaseAsync);
    group.MapDelete("/{receiptId}/purchases/{purchaseId}", RemovePurchaseAsync);
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
    string? sortBy,
    string? sortOrder,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    GetReceiptsQuery query = new(pageSize, pageNumber, search, after, before, minTotal, maxTotal, sortBy, sortOrder);
    return (await sender.SendAsync(query, cancellationToken))
      .Map(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> GetReceiptAsync(
    string receiptId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    return (await sender.SendAsync(new GetReceiptQuery(receiptId), cancellationToken))
      .Map(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> OpenNewReceiptAsync(
    OpenNewReceiptRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    return (await sender.SendAsync(request.MapToCommand(today), cancellationToken))
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
    var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    return (await sender.SendAsync(request.MapToCommand(receiptId, today), cancellationToken))
      .Map(r => r.MapTo<UpdateReceiptResponse>())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchaseAsync(
    string receiptId,
    AddPurchaseRequest request,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    return (await sender.SendAsync(request.MapToCommand(receiptId), cancellationToken))
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
    return (await sender.SendAsync(request.MapToCommand(receiptId, purchaseId), cancellationToken))
      .Map(r => r.MapTo<UpdatePurchaseResponse>())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> RemovePurchaseAsync(
    string receiptId,
    string purchaseId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    return (await sender.SendAsync(new RemovePurchaseCommand(receiptId, purchaseId), cancellationToken))
      .Match(Handle, _ => Results.NoContent());
  }

  private static async Task<IResult> DeleteReceiptAsync(
    string receiptId,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    return (await sender.SendAsync(new DeleteReceiptCommand(receiptId), cancellationToken))
      .Match(Handle, _ => Results.NoContent());
  }
}
