using System.Collections.Immutable;
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
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
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
        IDuplicateReceiptPersistence,
        IAddReceiptItemPersistence,
        IUpdateReceiptItemPersistence,
        IDeleteReceiptItemPersistence,
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
                            : Some.With(None.OfType<Money>())
                        from description in receiptItem.Description is not null
                            ? Some.With(new Description(receiptItem.Description))
                            : Some.With(None.OfType<Description>())
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

    public async Task<Result<Maybe<ReceiptDetails>>> GetReceiptByIdAsync(
        Guid receiptId,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        string receiptQuery = """
                              select id as "Id", store as "Store", purchase_date as "PurchaseDate"
                              from receipts
                              where id = @Id
                              """;

        ReceiptDto? receiptDto = await connection
            .QueryFirstOrDefaultAsync<ReceiptDto>(receiptQuery, new { Id = receiptId });

        if (receiptDto is null)
        {
            return None.OfType<ReceiptDetails>();
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
            .QueryAsync<ReceiptItemDto>(receiptItemsQuery, new { ReceiptId = receiptId });

        return Some.With(new ReceiptDetails(
            receiptDto.Id,
            receiptDto.Store,
            receiptDto.PurchaseDate,
            receiptItemDtos.Select(rItem => new ReceiptItemDetails(
                rItem.Id,
                rItem.Item,
                rItem.Category,
                rItem.UnitPrice,
                rItem.Quantity,
                rItem.Discount.HasValue ? Some.With(rItem.Discount.Value) : None.OfType<decimal>(),
                rItem.Description is not null ? Some.With(rItem.Description) : None.OfType<string>()))));
    }

    public async Task<Result<Unit>> SaveReceiptAsync(Receipt receipt, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using IDbTransaction transaction = connection.BeginTransaction();

        try
        {
            if (receipt is DeletedReceipt)
            {
                await connection.ExecuteAsync(
                    "delete from receipt_items where receipt_id = @Id",
                    new { Id = receipt.Id.Value },
                    transaction);

                await connection.ExecuteAsync(
                    "delete from receipts where id = @Id",
                    new { Id = receipt.Id.Value },
                    transaction);

                transaction.Commit();
                return Unit.Instance;
            }

            await connection.ExecuteAsync(
                """
                insert into receipts (id, store, purchase_date)
                values (@Id, @Store, @PurchaseDate)
                on conflict (id) do update
                  set store = excluded.store,
                    purchase_date = excluded.purchase_date;
                """,
                new { Id = receipt.Id.Value, Store = receipt.Store.Name, PurchaseDate = receipt.PurchaseDate.Date },
                transaction);

            HashSet<Guid> existingItemIds = (await connection.QueryAsync<Guid>(
                "select id from receipt_items where receipt_id = @ReceiptId",
                new { ReceiptId = receipt.Id.Value },
                transaction)).ToHashSet();

            HashSet<Guid> incomingItemIds = receipt.Items.Select(i => i.Id.Value).ToHashSet();

            Guid[] itemsToDelete = existingItemIds.Except(incomingItemIds).ToArray();
            if (itemsToDelete.Length != 0)
            {
                await connection.ExecuteAsync(
                    """
                    delete from receipt_items
                    where id = any(@Ids)
                    """,
                    new { Ids = itemsToDelete },
                    transaction);
            }

            foreach (var item in receipt.Items)
            {
                await connection.ExecuteAsync(
                    """
                        insert into receipt_items
                            (id, receipt_id, item, category, unit_price, quantity, discount, description)
                        values
                            (@Id, @ReceiptId, @Item, @Category, @UnitPrice, @Quantity, @Discount, @Description)
                        on conflict (id) do update set
                            item = excluded.item,
                            category = excluded.category,
                            unit_price = excluded.unit_price,
                            quantity = excluded.quantity,
                            discount = excluded.discount,
                            description = excluded.description;
                    """, new
                    {
                        Id = item.Id.Value,
                        ReceiptId = receipt.Id.Value,
                        Item = item.Item.Name,
                        Category = item.Category.Name,
                        UnitPrice = item.UnitPrice.Value,
                        Quantity = item.Quantity.Value,
                        Discount = item.Discount.Match<decimal?>(none: () => null, some: m => m.Value),
                        Description = item.Description.Match<string?>(none: () => null, some: m => m.Value)
                    }, transaction);
            }

            transaction.Commit();
            return Unit.Instance;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Failure.Fatal("Receipt.Save.Failed", ex.Message);
        }
    }

    public async Task<Result<PageOf<ReceiptSummary>>> GetReceiptsAsync(
        int pageSize,
        int skip,
        ReceiptOrder order,
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        List<string> whereClauses = [];
        DynamicParameters parameters = new();

        int idx = 0;
        foreach (ReceiptFilter f in filters)
        {
            if (f.Stores.Any())
            {
                whereClauses.Add($"r.store = any(@stores{idx})");
                parameters.Add($"stores{idx}", f.Stores.ToArray());
            }

            if (f.PurchaseDateFrom.HasValue)
            {
                whereClauses.Add($"r.purchase_date >= @pdateFrom{idx}");
                parameters.Add($"pdateFrom{idx}", f.PurchaseDateFrom.Value);
            }

            if (f.PurchaseDateTo.HasValue)
            {
                whereClauses.Add($"r.purchase_date <= @pdateTo{idx}");
                parameters.Add($"pdateTo{idx}", f.PurchaseDateTo.Value);
            }

            if (f.TotalMin.HasValue)
            {
                whereClauses.Add($"total >= @tmin{idx}");
                parameters.Add($"tmin{idx}", f.TotalMin.Value);
            }

            if (f.TotalMax.HasValue)
            {
                whereClauses.Add($"total <= @tmax{idx}");
                parameters.Add($"tmax{idx}", f.TotalMax.Value);
            }

            idx++;
        }

        string where = whereClauses.Count != 0 ? $"where {string.Join(" and ", whereClauses)}" : "";
        string sqlBase = $"""
            from receipts r
            left join (
                select
                    receipt_id,
                    sum(unit_price * quantity - coalesce(discount, 0)) as total
                from receipt_items
                group by receipt_id
            ) ri on ri.receipt_id = r.id
            {where}
            """;

        string sqlCount = $"SELECT COUNT(*) {sqlBase}";
        int totalCount = await connection.ExecuteScalarAsync<int>(sqlCount, parameters);
        string orderDir = order.Descending ? "DESC" : "ASC";
        string orderBy = order.OrderBy.ToUpperInvariant() switch
        {
            "ID" => "r.id",
            "STORE" => "r.store",
            "PURCHASEDATE" => "r.purchase_date",
            "TOTAL" => "total",
            _ => "r.id"
        };

        string sqlList = $@"
            select
                r.id as Id,
                r.store as Store,
                r.purchase_date as PurchaseDate,
                COALESCE(total, 0) as Total
            {sqlBase}
            order by {orderBy} {orderDir}
            limit @pageSize offset @skip
        ";
        parameters.Add("pageSize", pageSize);
        parameters.Add("skip", skip);

        IEnumerable<ReceiptSummary> list = await connection.QueryAsync<ReceiptSummary>(sqlList, parameters);
        return Page.Of(list.ToImmutableList(), (uint)totalCount);
    }

    public async Task<Result<decimal>> GetTotalCostAsync(
        IEnumerable<ReceiptFilter> filters,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        List<string> whereClauses = [];
        DynamicParameters parameters = new();

        int idx = 0;
        foreach (ReceiptFilter f in filters)
        {
            if (f.Stores.Any())
            {
                whereClauses.Add($"r.store = any(@stores{idx})");
                parameters.Add($"stores{idx}", f.Stores.ToArray());
            }

            if (f.PurchaseDateFrom.HasValue)
            {
                whereClauses.Add($"r.purchase_date >= @pdateFrom{idx}");
                parameters.Add($"pdateFrom{idx}", f.PurchaseDateFrom.Value);
            }

            if (f.PurchaseDateTo.HasValue)
            {
                whereClauses.Add($"r.purchase_date <= @pdateTo{idx}");
                parameters.Add($"pdateTo{idx}", f.PurchaseDateTo.Value);
            }

            if (f.TotalMin.HasValue)
            {
                whereClauses.Add($"total >= @tmin{idx}");
                parameters.Add($"tmin{idx}", f.TotalMin.Value);
            }

            if (f.TotalMax.HasValue)
            {
                whereClauses.Add($"total <= @tmax{idx}");
                parameters.Add($"tmax{idx}", f.TotalMax.Value);
            }

            idx++;
        }

        string where = whereClauses.Count != 0 ? $"where {string.Join(" and ", whereClauses)}" : "";
        string sql = $@"
            select coalesce(sum(ri.unit_price * ri.quantity - coalesce(ri.discount,0)), 0) as TotalCost
            from receipts r
            join receipt_items ri on ri.receipt_id = r.id
            {where}
        ";

        return await connection.ExecuteScalarAsync<decimal>(sql, parameters);
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