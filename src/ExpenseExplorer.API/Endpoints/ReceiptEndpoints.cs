using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    if (request.PurchaseDate > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime)) {
      return Results.BadRequest();
    }

    if (string.IsNullOrWhiteSpace(request.StoreName)) {
      return Results.BadRequest();
    }

    return Results.Ok(request);
  }
}
