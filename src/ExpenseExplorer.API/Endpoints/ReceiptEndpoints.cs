using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Validations;

namespace ExpenseExplorer.API.Endpoints;

public static class ReceiptEndpoints {
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder) {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    IEnumerable<ValidationError> errors = CollectErrors(request, timeProvider).ToList();
    return errors.Any()
      ? Handle(errors)
      : Results.Ok(request);
  }

  private static IEnumerable<ValidationError> CollectErrors(OpenNewReceiptRequest request, TimeProvider timeProvider) {
    IEnumerable<(Func<OpenNewReceiptRequest, bool> isInvalid, ValidationError error)> validations = [
      (r => string.IsNullOrWhiteSpace(r.StoreName),
        ValidationError.Create("StoreName", "EMPTY_STORE_NAME")),
      (r => r.PurchaseDate > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime),
        ValidationError.Create("PurchaseDate", "FUTURE_DATE"))
    ];

    return validations
      .Where(v => v.isInvalid(request))
      .Select(v => v.error);
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
