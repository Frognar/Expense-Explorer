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
      from receipt in ReceiptValidator.Validate(request.MapToCommand(today))
      select receipt.MapToResponse();

    return validatedResponse
      .Match(Handle, Results.Ok);
  }

  private static IResult AddPurchase(string receiptId, AddPurchaseRequest request)
  {
    Validated<object> validatedResponse =
      from response in PurchaseValidator.Validate(request.MapToCommand(receiptId))
      select response;

    return validatedResponse
      .Match(Handle, Results.Ok);
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
