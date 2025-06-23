using DotResult;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

public interface IGetReceiptSummariesPersistence
{
    public Task<Result<PageOf<ReceiptSummary>>> GetReceiptsAsync(
        int pageSize,
        int skip,
        ReceiptOrder order,
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken);

    public Task<Result<decimal>> GetTotalCostAsync(
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken);
}