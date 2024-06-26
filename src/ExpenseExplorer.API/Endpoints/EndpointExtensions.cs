namespace ExpenseExplorer.API.Endpoints;

public static class EndpointExtensions
{
  public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    return endpointRouteBuilder
      .MapReceiptEndpoints()
      .MapStoreEndpoints()
      .MapItemEndpoints()
      .MapCategoryEndpoints()
      .MapSourceEndpoints()
      .MapReportEndpoints()
      .MapIncomeEndpoints();
  }
}
