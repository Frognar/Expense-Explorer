using System.Data;
using Dapper;
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
using ExpenseExplorer.Infrastructure.Database;

namespace ExpenseExplorer.Infrastructure.Features.Receipts;

internal sealed class ReceiptRepository(IDbConnectionFactory connectionFactory)
    : ICreateReceiptHeaderPersistence,
        IUpdateReceiptHeaderPersistence,
        IDeleteReceiptHeaderPersistence,
        IAddReceiptItemPersistence,
        IUpdateReceiptItemPersistence,
        IReceiptItemDeletePersistence,
        IGetReceiptByIdPersistence,
        IGetReceiptSummariesPersistence
{
    public async Task<Result<Unit>> SaveNewReceiptHeaderAsync(Receipt receipt, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        string query = """
                       insert into receipts (id, store_name, purchase_date)
                       values (@Id, @Store, @PurchaseDate)
                       """;

        object parameters = new
        {
            Id = receipt.Id.Value,
            Store = receipt.Store.Name,
            PurchaseDate = receipt.PurchaseDate.Date
        };

        int result = await connection.ExecuteAsync(query, parameters);
        return result > 0 ? Unit.Instance : Failure.Fatal("Receipt.SaveNewReceiptHeader.Failed");
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