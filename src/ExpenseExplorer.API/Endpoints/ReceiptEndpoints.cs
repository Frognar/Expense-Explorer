using ExpenseExplorer.API.Contract;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", (OpenNewReceiptRequest r) => Results.Ok(r));
    return endpointRouteBuilder;
  }
}
