using DotResult;
using ExpenseExplorer.Application.Domain.Receipts;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.AddItem;

public interface IAddReceiptItemPersistence
{
    Task<Result<Receipt>> GetReceiptByIdAsync(ReceiptId receiptId, CancellationToken cancellationToken);
    Task<Result<Unit>> SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken);
}