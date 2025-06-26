using DotResult;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.CreateHeader;

public interface ICreateReceiptHeaderPersistence
{
    Task<Result<Unit>> SaveNewReceiptHeaderAsync(Receipt receipt, CancellationToken cancellationToken);
}