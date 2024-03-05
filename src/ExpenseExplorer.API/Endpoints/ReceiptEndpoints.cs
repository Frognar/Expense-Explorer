using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    Func<string, DateOnly, OpenNewReceiptResponse> createRequest = (storeName, purchaseDate)
      => new OpenNewReceiptResponse(Guid.NewGuid().ToString("N"), storeName, purchaseDate);

    return createRequest
      .Apply(Validate(request.StoreName))
      .Apply(Validate(request.PurchaseDate, timeProvider))
      .Match(
        Handle,
        Results.Ok
      );
  }

  private static Validated<string> Validate(string storeName) {
    return string.IsNullOrWhiteSpace(storeName)
      ? Validation.Failed<string>([ValidationError.Create("StoreName", "EMPTY_STORE_NAME")])
      : Validation.Succeeded(storeName.Trim());
  }

  private static Validated<DateOnly> Validate(DateOnly purchaseDate, TimeProvider timeProvider) {
    return purchaseDate > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime)
      ? Validation.Failed<DateOnly>([ValidationError.Create("PurchaseDate", "FUTURE_DATE")])
      : Validation.Succeeded(purchaseDate);
  }

  private static IResult Handle(IEnumerable<ValidationError> errors) {
    return Results.BadRequest(
      new {
        Errors = errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)
          )
      }
    );
  }
}
