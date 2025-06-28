using DotResult;

namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public interface IGetReceiptItemsPersistence
{
    public Task<Result<PageOf<ReceiptItemDetails>>> GetReceiptItemsAsync(
        int pageSize,
        int skip,
        ReceiptItemOrder order,
        IEnumerable<ReceiptItemFilter> filters,
        CancellationToken cancellationToken);

    public Task<Result<decimal>> GetTotalCostAsync(
        IEnumerable<ReceiptItemFilter> filters,
        CancellationToken cancellationToken);
}