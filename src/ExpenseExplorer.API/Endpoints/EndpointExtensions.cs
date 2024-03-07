namespace ExpenseExplorer.API.Endpoints;

public static class EndpointExtensions
{
  public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    return endpointRouteBuilder.MapReceiptEndpoints();
  }
}
