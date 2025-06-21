using DotResult;
using ExpenseExplorer.Application.Domain.Receipts;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.DeleteHeader;

public interface IDeleteReceiptHeaderPersistence
{
    Task<Result<Receipt>> GetReceiptByIdAsync(ReceiptId receiptId, CancellationToken cancellationToken);
    Task<Result<Unit>> SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken);
}