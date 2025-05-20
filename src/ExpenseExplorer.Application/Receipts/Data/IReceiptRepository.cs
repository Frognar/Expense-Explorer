using ExpenseExplorer.Application.Receipts.DTO;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IReceiptRepository
{
    public Task<Unit> CreateReceipt(CreateReceiptRequest receipt, CancellationToken cancellationToken);
}