namespace ExpenseExplorer.API.Endpoints;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Application.Validations;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapPost("/", OpenNewReceipt);
    group.MapPost("/{receiptId}", AddPurchase);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    Validated<OpenNewReceiptResponse> validatedResponse =
      from receipt in ReceiptValidator.Validate(request.StoreName, request.PurchaseDate, today)
      select receipt.MapToResponse();

    return validatedResponse
      .Match(Handle, Results.Ok);
  }

  private static IResult AddPurchase(string receiptId, AddPurchaseRequest request)
  {
    return PurchaseValidator.Validate(request.ProductName, request.ProductCategory, request.Quantity, request.UnitPrice)
      .Match(Handle, _ => Results.Ok(new { receiptId, request }));
  }

  private static IResult Handle(IEnumerable<ValidationError> errors)
  {
    return Results.BadRequest(
      new
      {
        Errors = errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)),
      });
  }
}
