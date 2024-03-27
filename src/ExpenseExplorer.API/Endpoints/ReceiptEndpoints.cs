namespace ExpenseExplorer.API.Endpoints;

using System.Diagnostics;
using System.Net;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapPost("/", OpenNewReceipt);
    group.MapPost("/{receiptId}", AddPurchase);
    return endpointRouteBuilder;
  }

  private static async Task<IResult> OpenNewReceipt(
    OpenNewReceiptRequest request,
    TimeProvider timeProvider,
    IReceiptRepository repository,
    CancellationToken cancellationToken = default)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    OpenNewReceiptCommandHandler handler = new(repository);
    Either<Failure, Receipt> result = await handler.HandleAsync(request.MapToCommand(today), cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchase(
    string receiptId,
    AddPurchaseRequest request,
    IReceiptRepository repository,
    CancellationToken cancellationToken = default)
  {
    AddPurchaseCommandHandler handler = new(repository);
    Either<Failure, Receipt> result = await handler.HandleAsync(request.MapToCommand(receiptId), cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static IResult Handle(Failure failure)
  {
    return failure switch
    {
      ValidationFailure validationFailure => HandleValidation(validationFailure),
      NotFoundFailure notFoundFailure => HandleNotFound(notFoundFailure),
      FatalFailure fatal => Results.Problem(detail: fatal.Message, statusCode: (int)HttpStatusCode.InternalServerError),
      _ => throw new UnreachableException(),
    };
  }

  private static IResult HandleValidation(ValidationFailure validationFailure)
  {
    return Results.Problem(
      detail: validationFailure.Message,
      statusCode: (int)HttpStatusCode.BadRequest,
      extensions: new Dictionary<string, object?>
      {
        ["Errors"] = validationFailure.Errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)),
      });
  }

  private static IResult HandleNotFound(NotFoundFailure notFoundFailure)
  {
    return Results.Problem(
      detail: notFoundFailure.Message,
      statusCode: (int)HttpStatusCode.NotFound,
      extensions: new Dictionary<string, object?> { ["Id"] = notFoundFailure.Id, });
  }
}
