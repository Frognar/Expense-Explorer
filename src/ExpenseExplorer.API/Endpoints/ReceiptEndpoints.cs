using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request) {
    if (request.PurchaseDate > DateOnly.FromDateTime(DateTime.Now)) {
      return Results.BadRequest();
    }

    return Results.Ok(request);
  }
}
