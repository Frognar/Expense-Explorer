namespace ExpenseExplorer.API.Endpoints;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Application.Validations;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    Validated<OpenNewReceiptResponse> validatedResponse =
      from receipt in ReceiptValidator.Validate(request.StoreName, request.PurchaseDate, today)
      select receipt.MapToResponse();

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
