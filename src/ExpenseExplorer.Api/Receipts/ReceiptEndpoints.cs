using System.Text;
using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Api.Receipts.Import;
using ExpenseExplorer.Api.Receipts.Import.Photo;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Receipts;

internal static class ReceiptEndpoints
{
    private const long MaxImportFileBytes = 1024 * 1024;
    private const long MaxPhotoBytes = 20 * 1024 * 1024;

    public static RouteGroupBuilder MapReceipts(this RouteGroupBuilder api)
    {
        RouteGroupBuilder receipts = api.MapGroup("/receipts").WithTags("Receipts");

        receipts.MapGet("/", ListAsync);
        receipts.MapGet("/{receiptId:guid}", GetAsync).WithName(nameof(GetAsync));
        receipts.MapGet("/{receiptId:guid}/export.csv", ExportAsync);
        receipts.MapPost("/", CreateAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPatch("/{receiptId:guid}", ChangeAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapDelete("/{receiptId:guid}", DeleteAsync).RequireAuthorization(Policies.CanEdit);
        receipts.MapPost("/import/biedronka", ImportBiedronkaAsync)
            .RequireAuthorization(Policies.CanEdit)
            .DisableAntiforgery(); // Requests carry a bearer token, which a forged cross-site form cannot add.
        receipts.MapPost("/import/photo", ImportPhotoAsync)
            .RequireAuthorization(Policies.CanEdit)
            .DisableAntiforgery();
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

    /// <summary>UTF-8 with a byte order mark, so spreadsheets show Polish letters correctly.</summary>
    private static async Task<IResult> ExportAsync(Guid receiptId, IReceiptQueries queries, CancellationToken cancellationToken) =>
        await queries.GetAsync(receiptId, cancellationToken) is { } receipt
            ? Results.File(
                [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(ReceiptCsv.Write(receipt))],
                "text/csv; charset=utf-8",
                ReceiptCsv.FileName(receipt))
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

    /// <summary>Takes the JSON e-receipt as a multipart form file named "file".</summary>
    private static async Task<IResult> ImportBiedronkaAsync(
        IFormFile file,
        IReceiptRepository receipts,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        if (file.Length > MaxImportFileBytes)
        {
            return ErrorResults.From([new Error("Import.FileTooLarge", "The file is larger than 1 MB.", Target: "file")]);
        }

        using StreamReader reader = new(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        string json = await reader.ReadToEndAsync(cancellationToken);

        return await BiedronkaReceiptParser.Parse(json, clock.Today(), clock.LocalTimeZone)
            .ToHttpAsync(command => ReceiptUseCases.ImportAsync(receipts, command, cancellationToken), Created);
    }

    /// <summary>Takes a photo of a paper receipt as a multipart form file named "file".</summary>
    private static async Task<IResult> ImportPhotoAsync(
        IFormFile file,
        IReceiptOcr ocr,
        IReceiptRepository receipts,
        TimeProvider clock,
        ILogger<IReceiptOcr> logger,
        CancellationToken cancellationToken)
    {
        if (file.Length > MaxPhotoBytes)
        {
            return ErrorResults.From([new Error("Import.PhotoTooLarge", "The photo is larger than 20 MB.", Target: "file")]);
        }

        await using Stream photo = file.OpenReadStream();
        Result<IReadOnlyList<OcrText>> read = await ocr.ReadAsync(photo, file.ContentType, cancellationToken);

        return await read
            .Map(PrintedLines.Of)
            .Bind(lines =>
            {
                logger.LinesRead(lines.Count, lines);
                return PaperReceiptParser.Parse(lines, clock.Today());
            })
            .ToHttpAsync(
                async paper => (await ReceiptUseCases.ImportAsync(receipts, paper.Receipt, cancellationToken))
                    .Map(receipt => (Receipt: receipt, Paper: paper)),
                imported => Results.Created(
                    $"{Routing.ApiPrefix}/receipts/{imported.Receipt.Id.Value}",
                    new PhotoImportResponse(
                        ReceiptResponses.From(imported.Receipt),
                        imported.Paper.PrintedTotal,
                        imported.Paper.PurchaseDateFound,
                        imported.Paper.SkippedLines)));
    }

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
