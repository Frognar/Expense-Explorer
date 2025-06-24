using DotMaybe;
using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Domain.Receipts;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Infrastructure.Features.Receipts;

internal sealed class ReceiptRepository
    : ICreateReceiptHeaderPersistence,
        IUpdateReceiptHeaderPersistence,
        IDeleteReceiptHeaderPersistence,
        IAddReceiptItemPersistence,
        IUpdateReceiptItemPersistence,
        IReceiptItemDeletePersistence,
        IGetReceiptByIdPersistence,
        IGetReceiptSummariesPersistence
{
    public Task<Result<Unit>> SaveNewReceiptHeaderAsync(Receipt receipt, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Receipt>> GetReceiptByIdAsync(ReceiptId receiptId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Maybe<ReceiptDetails>>> GetReceiptByIdAsync(Guid receiptId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Unit>> SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PageOf<ReceiptSummary>>> GetReceiptsAsync(
        int pageSize,
        int skip,
        ReceiptOrder order,
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<decimal>> GetTotalCostAsync(
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}