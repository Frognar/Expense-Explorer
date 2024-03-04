using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    if (request.PurchaseDate > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime)) {
      return Results.BadRequest(new ValidationError("PurchaseDate", "FUTURE_DATE"));
    }

    if (string.IsNullOrWhiteSpace(request.StoreName)) {
      return Results.BadRequest(new ValidationError("StoreName", "EMPTY_STORE_NAME"));
    }

    return Results.Ok(request);
  }
}
