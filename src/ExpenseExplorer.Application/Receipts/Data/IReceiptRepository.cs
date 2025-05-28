using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.DTO;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IReceiptRepository
{
    public Task<Unit> CreateReceipt(CreateReceiptRequest receipt, CancellationToken cancellationToken);

    public Task<PageOf<ReceiptSummary>> GetReceiptsAsync(
        int pageSize,
        int skip,
        ReceiptOrder order,
        ImmutableArray<ReceiptFilter> filters,
        CancellationToken cancellationToken);

    public Task<Money> GetTotalCostAsync(
        ImmutableArray<ReceiptFilter> filters,
        CancellationToken cancellationToken);

    public Task<Option<ReceiptDetails>> GetReceiptByIdAsync(ReceiptId id, CancellationToken cancellationToken);
    public Task<ImmutableArray<Store>> GetStoresAsync(Option<string> search, CancellationToken cancellationToken);
    public Task<ImmutableArray<Item>> GetItemsAsync(Option<string> search, CancellationToken cancellationToken);
}