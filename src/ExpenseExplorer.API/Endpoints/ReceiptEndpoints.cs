using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    List<ValidationError> errors = [];
    if (request.PurchaseDate > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime)) {
      errors.Add(new ValidationError("PurchaseDate", "FUTURE_DATE"));
    }

    if (string.IsNullOrWhiteSpace(request.StoreName)) {
      errors.Add(new ValidationError("StoreName", "EMPTY_STORE_NAME"));
    }

    return errors.Any()
      ? Results.BadRequest(errors)
      : Results.Ok(request);
  }
}
