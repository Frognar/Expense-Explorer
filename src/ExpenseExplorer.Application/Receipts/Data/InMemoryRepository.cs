using ExpenseExplorer.Application.Receipts.DTO;

namespace ExpenseExplorer.Application.Receipts.Data;

public sealed class InMemoryRepository : IReceiptRepository
{
    private static readonly List<ReceiptDetails> Receipts = [];

    public Task<Unit> CreateReceipt(CreateReceiptRequest receipt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        Receipts.Add(new ReceiptDetails(receipt.Store, receipt.PurchaseDate, []));
        return Task.FromResult(Unit.Instance);
    }
}