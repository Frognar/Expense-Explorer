using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public record GetReceiptItemsQuery(
    int PageSize,
    int Skip,
    ReceiptItemOrder Order,
    IEnumerable<ReceiptItemFilter> Filters)
    : IQuery<GetReceiptItemsResponse>
{
    public static GetReceiptItemsQuery Default =>
        new(20, 0, ReceiptItemOrder.Id with { Descending = true }, []);

    public GetReceiptItemsQuery Where(ReceiptItemFilter filter) =>
        this with { Filters = Filters.Append(filter) };

    public GetReceiptItemsQuery OrderBy(ReceiptItemOrder order) =>
        this with { Order = order };

    public GetReceiptItemsQuery OrderByDescending(ReceiptItemOrder order) =>
        this with { Order = order with { Descending = true } };

    public GetReceiptItemsQuery GetPage(int pageSize, int skip) =>
        this with { PageSize = pageSize, Skip = skip };
}