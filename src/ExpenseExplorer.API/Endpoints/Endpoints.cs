namespace ExpenseExplorer.API.Endpoints;

public static class Endpoints {
  public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    return endpointRouteBuilder.MapReceiptEndpoints();
  }
}
