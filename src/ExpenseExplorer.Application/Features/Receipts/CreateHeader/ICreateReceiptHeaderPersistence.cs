using DotResult;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public interface ICreateReceiptHeaderPersistence
{
    Task<Result<Unit>> SaveNewReceiptHeaderAsync(
        ReceiptId id,
        Store store,
        PurchaseDate purchaseDate,
        CancellationToken cancellationToken);
}