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

    public async Task<Result<Receipt>> GetReceiptByIdAsync(ReceiptId receiptId, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        string receiptQuery = """
                              select id as "Id", store as "Store", purchase_date as "PurchaseDate"
                              from receipts
                              where id = @Id
                              """;

        ReceiptDto? receiptDto = await connection
            .QueryFirstOrDefaultAsync<ReceiptDto>(receiptQuery, new { Id = receiptId.Value });

        if (receiptDto is null)
        {
            return Failure.NotFound(code: "Receipt.GetById.NotFound", message: "Receipt not found");
        }

        string receiptItemsQuery = """
                                   select
                                      id as "Id"
                                    , receipt_id as "ReceiptId"
                                    , item as "Item"
                                    , category as "Category"
                                    , unit_price as "UnitPrice"
                                    , quantity as "Quantity"
                                    , discount as "Discount"
                                    , description as "Description"
                                   from receipt_items
                                   where receipt_id = @ReceiptId
                                   """;

        IEnumerable<ReceiptItemDto> receiptItemDtos = await connection
            .QueryAsync<ReceiptItemDto>(receiptItemsQuery, new { ReceiptId = receiptId.Value });

        return (
                from rId in ReceiptId.TryCreate(receiptDto.Id)
                from store in Store.TryCreate(receiptDto.Store)
                from purchaseDate in PurchaseDate.TryCreate(receiptDto.PurchaseDate, receiptDto.PurchaseDate)
                select new Receipt(rId, store, purchaseDate, []))
            .Bind(r =>
                receiptItemDtos.Select(receiptItem =>
                        from id in ReceiptItemId.TryCreate(receiptItem.Id)
                        from relatedReceiptId in ReceiptId.TryCreate(receiptItem.ReceiptId)
                        from item in Item.TryCreate(receiptItem.Item)
                        from category in Category.TryCreate(receiptItem.Category)
                        from unitPrice in Money.TryCreate(receiptItem.UnitPrice)
                        from quantity in Quantity.TryCreate(receiptItem.Quantity)
                        from discount in receiptItem.Discount.HasValue
                            ? Some.With(Money.TryCreate(receiptItem.Discount.Value))
                            : None.OfType<Money>()
                        from description in receiptItem.Description is not null
                            ? Some.With(new Description(receiptItem.Description))
                            : None.OfType<Description>()
                        select new ReceiptItem(
                            id,
                            relatedReceiptId,
                            item,
                            category,
                            quantity,
                            unitPrice,
                            discount,
                            description)
                    )
                    .TraverseMaybeToImmutableList()
                    .Map(items => r with { Items = items }))
            .ToResult(() =>
                Failure.Custom(code: "Receipt.GetById.Invalid", message: "Invalid receipt", type: "InvalidReceipt"));
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

    private sealed record ReceiptDto(
        Guid Id,
        string Store,
        DateOnly PurchaseDate);

    private sealed record ReceiptItemDto(
        Guid Id,
        Guid ReceiptId,
        string Item,
        string Category,
        decimal UnitPrice,
        decimal Quantity,
        decimal? Discount,
        string? Description);
}