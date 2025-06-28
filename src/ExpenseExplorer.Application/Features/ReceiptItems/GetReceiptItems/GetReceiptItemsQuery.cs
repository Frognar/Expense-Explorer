using ExpenseExplorer.Application.Abstractions.Messaging;

namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public record GetReceiptItemsQuery(
    int PageSize,
    int Skip,
    ReceiptItemOrder Order,
    IEnumerable<ReceiptItemFilter> Filters)
    : IQuery<GetReceiptItemsResponse>;