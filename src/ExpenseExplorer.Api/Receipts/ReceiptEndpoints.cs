using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Receipts;

internal static class ReceiptEndpoints
{
    public static RouteGroupBuilder MapReceipts(this RouteGroupBuilder api)
    {
        RouteGroupBuilder receipts = api.MapGroup("/receipts").WithTags("Receipts");

        receipts.MapGet("/", ListAsync);
        receipts.MapGet("/{receiptId:guid}", GetAsync).WithName(nameof(GetAsync));
        receipts.MapPost("/", CreateAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPatch("/{receiptId:guid}", ChangeAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapDelete("/{receiptId:guid}", DeleteAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPost("/{receiptId:guid}/duplicate", DuplicateAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPost("/{receiptId:guid}/items", AddItemAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPut("/{receiptId:guid}/items/{itemId:guid}", ChangeItemAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapDelete("/{receiptId:guid}/items/{itemId:guid}", RemoveItemAsync).RequireAuthorization(Policies.CanEdit);

        return api;
    }

    private static Task<IResult> ListAsync(
        [AsParameters] ReceiptListRequest request,
        IReceiptQueries queries,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseList(request)
            .ToHttpAsync(
                async query => Result.Success(await queries.ListAsync(query, cancellationToken)),
                list => Results.Ok(list));

    private static async Task<IResult> GetAsync(Guid receiptId, IReceiptQueries queries, CancellationToken cancellationToken) =>
        await queries.GetAsync(receiptId, cancellationToken) is { } receipt
            ? Results.Ok(receipt)
            : ErrorResults.From([ApplicationErrors.ReceiptNotFound]);

    private static Task<IResult> CreateAsync(
        CreateReceiptRequest request,
        IReceiptRepository receipts,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseCreate(request, clock.Today())
            .ToHttpAsync(command => ReceiptUseCases.CreateAsync(receipts, command, cancellationToken), Created);

    private static Task<IResult> ChangeAsync(
        Guid receiptId,
        UpdateReceiptRequest request,
        IReceiptRepository receipts,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseChange(receiptId, request, clock.Today())
            .ToHttpAsync(
                command => ReceiptUseCases.ChangeAsync(receipts, command, cancellationToken),
                receipt => Results.Ok(ReceiptResponses.From(receipt)));

    private static Task<IResult> DeleteAsync(Guid receiptId, IReceiptRepository receipts, CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseDelete(receiptId)
            .ToHttpAsync(
                command => ReceiptUseCases.DeleteAsync(receipts, command, cancellationToken),
                _ => Results.NoContent());

    private static Task<IResult> DuplicateAsync(
        Guid receiptId,
        DuplicateReceiptRequest request,
        IReceiptRepository receipts,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseDuplicate(receiptId, request, clock.Today())
            .ToHttpAsync(command => ReceiptUseCases.DuplicateAsync(receipts, command, cancellationToken), Created);

    private static Task<IResult> AddItemAsync(
        Guid receiptId,
        ReceiptItemRequest request,
        IReceiptRepository receipts,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseAddItem(receiptId, request)
            .ToHttpAsync(
                command => ReceiptUseCases.AddItemAsync(receipts, command, cancellationToken),
                item => Results.Created(
                    $"{Routing.ApiPrefix}/receipts/{receiptId}",
                    ReceiptResponses.From(item)));

    private static Task<IResult> ChangeItemAsync(
        Guid receiptId,
        Guid itemId,
        ReceiptItemRequest request,
        IReceiptRepository receipts,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseChangeItem(receiptId, itemId, request)
            .ToHttpAsync(
                command => ReceiptUseCases.ChangeItemAsync(receipts, command, cancellationToken),
                item => Results.Ok(ReceiptResponses.From(item)));

    private static Task<IResult> RemoveItemAsync(
        Guid receiptId,
        Guid itemId,
        IReceiptRepository receipts,
        CancellationToken cancellationToken) =>
        ReceiptRequestParser.ParseRemoveItem(receiptId, itemId)
            .ToHttpAsync(
                command => ReceiptUseCases.RemoveItemAsync(receipts, command, cancellationToken),
                _ => Results.NoContent());

    private static IResult Created(Receipt receipt) =>
        Results.Created($"{Routing.ApiPrefix}/receipts/{receipt.Id.Value}", ReceiptResponses.From(receipt));
}
