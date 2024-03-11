namespace ExpenseExplorer.API.Endpoints;

using System.Diagnostics;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapPost("/", OpenNewReceipt);
    group.MapPost("/{receiptId}", AddPurchase);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> OpenNewReceipt(OpenNewReceiptRequest request, TimeProvider timeProvider)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    Validated<Receipt> validated = ReceiptValidator.Validate(request.MapToCommand(today));
    if (validated.IsValid)
    {
      Receipt receipt = validated.Match(_ => throw new UnreachableException(), r => r);
      InMemoryEventStore eventStore = new();
      await eventStore.SaveEvents(receipt.Id, receipt.UnsavedChanges).ConfigureAwait(false);
    }

    return validated.Select(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchase(string receiptId, AddPurchaseRequest request)
  {
    Validated<object> validatedResponse =
      from response in PurchaseValidator.Validate(request.MapToCommand(receiptId))
      select response;

    if (validatedResponse.IsValid)
    {
      InMemoryEventStore eventStore = new();
      IEnumerable<Fact> events = await eventStore.GetEvents(Id.Create(receiptId)).ConfigureAwait(false);
      if (!events.Any())
      {
        return Results.NotFound();
      }
    }

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
