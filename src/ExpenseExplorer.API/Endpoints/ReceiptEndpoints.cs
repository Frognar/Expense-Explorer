namespace ExpenseExplorer.API.Endpoints;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/receipts", OpenNewReceipt);
    return endpointRouteBuilder;
  }

  private static IResult OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider)
  {
    Func<Store, PurchaseDate, Receipt> createReceipt = Receipt.New;

    return createReceipt
      .Apply(Validate(request.StoreName))
      .Apply(Validate(request.PurchaseDate, timeProvider))
      .Match(
        Handle,
        r => Results.Ok(r.MapToResponse()));
  }

  private static Validated<Store> Validate(string storeName)
  {
    return string.IsNullOrWhiteSpace(storeName)
      ? Validation.Failed<Store>([ValidationError.Create("StoreName", "EMPTY_STORE_NAME")])
      : Validation.Succeeded(Store.Create(storeName));
  }

  private static Validated<PurchaseDate> Validate(DateOnly purchaseDate, TimeProvider timeProvider)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    return purchaseDate > today
      ? Validation.Failed<PurchaseDate>([ValidationError.Create("PurchaseDate", "FUTURE_DATE")])
      : Validation.Succeeded(PurchaseDate.Create(purchaseDate, today));
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
