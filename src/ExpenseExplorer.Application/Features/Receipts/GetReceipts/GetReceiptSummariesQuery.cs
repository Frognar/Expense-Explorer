using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

public sealed record GetReceiptSummariesQuery(
    int PageSize,
    int Skip,
    ReceiptOrder Order,
    IEnumerable<ReceiptFilter> Filters)
    : IQuery<GetReceiptSummariesResponse>
{
    public static GetReceiptSummariesQuery Default =>
        new(20, 0, ReceiptOrder.Id with { Descending = true }, []);

    public GetReceiptSummariesQuery Where(ReceiptFilter filter) =>
        this with { Filters = Filters.Append(filter) };

    public GetReceiptSummariesQuery OrderBy(ReceiptOrder order) =>
        this with { Order = order };

    public GetReceiptSummariesQuery OrderByDescending(ReceiptOrder order) =>
        this with { Order = order with { Descending = true } };

    public GetReceiptSummariesQuery GetPage(int pageSize, int skip) =>
        this with { PageSize = pageSize, Skip = skip };
}