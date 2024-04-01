namespace ExpenseExplorer.API.Endpoints;

using System.Diagnostics;
using System.Net;
using CommandHub.Commands;
using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Mappers;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Domain.Receipts;

public static class ReceiptEndpoints
{
  public static IEndpointRouteBuilder MapReceiptEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
  {
    RouteGroupBuilder group = endpointRouteBuilder.MapGroup("/api/receipts");
    group.MapGet("/", GetReceiptsAsync);
    group.MapPost("/", OpenNewReceiptAsync);
    group.MapPost("/{receiptId}", AddPurchaseAsync);
    return endpointRouteBuilder;
  }

  private static Task<IResult> GetReceiptsAsync()
  {
    return Task.FromResult(Results.Ok());
  }

  private static async Task<IResult> OpenNewReceiptAsync(
    OpenNewReceiptRequest request,
    TimeProvider timeProvider,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    DateOnly today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    Either<Failure, Receipt> result = await sender.SendAsync(request.MapToCommand(today), cancellationToken);
    return result
      .MapRight(r => r.MapToResponse())
      .Match(Handle, Results.Ok);
  }

  private static async Task<IResult> AddPurchaseAsync(
    string receiptId,
    AddPurchaseRequest request,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    Either<Failure, Receipt> result = await sender.SendAsync(request.MapToCommand(receiptId), cancellationToken);
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
