using DotResult;
using ExpenseExplorer.Application.Domain.Receipts;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteItem;

public interface IDeleteReceiptItemPersistence
{
    Task<Result<Receipt>> GetReceiptByIdAsync(ReceiptId receiptId, CancellationToken cancellationToken);
    Task<Result<Unit>> SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken);
}