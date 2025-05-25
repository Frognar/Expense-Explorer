using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetReceiptsQuery(
    int PageSize,
    int Skip,
    ReceiptOrder Order,
    ImmutableArray<ReceiptFilter> Filters)
{
    public static GetReceiptsQuery Default =>
        new(20, 0, ReceiptOrder.Id with { Descending = true }, []);

    public GetReceiptsQuery Where(ReceiptFilter filter) =>
        this with { Filters = Filters.Add(filter) };

    public GetReceiptsQuery OrderBy(ReceiptOrder order) =>
        this with { Order = order };

    public GetReceiptsQuery OrderByDescending(ReceiptOrder order) =>
        this with { Order = order with { Descending = true } };

    public GetReceiptsQuery GetPage(int pageSize, int skip) =>
        this with { PageSize = pageSize, Skip = skip };
}

public sealed class GetReceiptsHandler(IReceiptRepository receiptRepository)
{
    public async Task<(PageOf<ReceiptSummary>, Money)> HandleAsync(GetReceiptsQuery query,
        CancellationToken cancellationToken)
    {
        PageOf<ReceiptSummary> pageOfReceipts = await receiptRepository
            .GetReceiptsAsync(
                query.PageSize,
                query.Skip,
                query.Order,
                query.Filters,
                cancellationToken);

        Money total = await receiptRepository.GetTotalCostAsync(query.Filters, cancellationToken);
        return (pageOfReceipts, total);
    }
}