using DotMaybe;
using DotResult;

namespace ExpenseExplorer.Application.Features.Receipts.GetReceipt;

public interface IGetReceiptByIdPersistence
{
    Task<Result<Maybe<ReceiptDetails>>> GetReceiptByIdAsync(Guid receiptId, CancellationToken cancellationToken);
}