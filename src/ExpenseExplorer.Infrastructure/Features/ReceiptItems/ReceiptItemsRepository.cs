using System.Collections.Immutable;
using System.Data;
using Dapper;
using DotMaybe;
using DotResult;
using ExpenseExplorer.Application;
using ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;
using ExpenseExplorer.Infrastructure.Database;

namespace ExpenseExplorer.Infrastructure.Features.ReceiptItems;

internal sealed class ReceiptItemsRepository(IDbConnectionFactory connectionFactory)
    : IGetReceiptItemsPersistence
{
    public async Task<Result<PageOf<ReceiptItemDetails>>> GetReceiptItemsAsync(
        int pageSize,
        int skip,
        ReceiptItemOrder order,
        IEnumerable<ReceiptItemFilter> filters,
        CancellationToken cancellationToken)
    {
        (List<string> whereClauses, DynamicParameters whereParameters) = BuildWhereClausesAndParameters(filters);
        string where = whereClauses.Count != 0 ? $"where {string.Join(" and ", whereClauses)}" : "";
        string sqlBase = $"""
                          from receipt_items ri
                          join receipts r on ri.receipt_id = r.id
                          {where}
                          """;

        string sqlCount = $"SELECT COUNT(*) {sqlBase}";
        string orderDir = order.Descending ? "DESC" : "ASC";
        string orderBy = order.OrderBy.ToUpperInvariant() switch
        {
            "ID" => "ri.id",
            "STORE" => "r.store",
            "PURCHASEDATE" => "r.purchase_date",
            "ITEM" => "ri.item",
            "CATEGORY" => "ri.category",
            "UNITPRICE" => "ri.unit_price",
            "QUANTITY" => "ri.quantity",
            "DISCOUNT" => "ri.discount",
            "DESCRIPTION" => "ri.description",
            "TOTAL" => "(ri.unit_price * ri.quantity - coalesce(ri.discount, 0))",
            _ => "ri.id"
        };

        string sqlList = $"""
                          select
                              ri.id as "Id",
                              ri.receipt_id as "ReceiptId",
                              r.store as "Store",
                              r.purchase_date as "PurchaseDate",
                              ri.item as "Item",
                              ri.category as "Category",
                              ri.unit_price as "UnitPrice",
                              ri.quantity as "Quantity",
                              ri.discount as "Discount",
                              ri.description as "Description"
                          {sqlBase}
                          order by {orderBy} {orderDir}
                          limit @pageSize offset @skip
                          """;

        DynamicParameters fullParameters = new(whereClauses);
        fullParameters.Add("pageSize", pageSize);
        fullParameters.Add("skip", skip);

        try
        {
            using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            int totalCount = await connection.ExecuteScalarAsync<int>(sqlCount, whereParameters);
            IEnumerable<ReceiptItemDto> list = await connection.QueryAsync<ReceiptItemDto>(sqlList, fullParameters);
            return Page.Of(list
                .Select(i => new ReceiptItemDetails(
                    i.Id,
                    i.ReceiptId,
                    i.Store,
                    i.PurchaseDate,
                    i.Item,
                    i.Category,
                    i.UnitPrice,
                    i.Quantity,
                    i.Discount.HasValue ? Some.With(i.Discount.Value) : None.OfType<decimal>(),
                    i.Description is not null ? Some.With(i.Description) : None.OfType<string>()))
                .ToImmutableList(), (uint)totalCount);
        }
        catch (Exception ex)
        {
            return Failure.Fatal(
                code: "DB_EXCEPTION",
                message: ex.Message,
                metadata: new Dictionary<string, object> { { "StackTrace", ex.StackTrace ?? "" } });
        }
    }

    public async Task<Result<decimal>> GetTotalCostAsync(
        IEnumerable<ReceiptItemFilter> filters,
        CancellationToken cancellationToken)
    {
        (List<string> whereClauses, DynamicParameters parameters) = BuildWhereClausesAndParameters(filters);
        string where = whereClauses.Count != 0 ? $"where {string.Join(" and ", whereClauses)}" : "";
        string sql = $"""
                      select coalesce(sum(ri.unit_price * ri.quantity - coalesce(ri.discount, 0)), 0) as TotalCost
                      from receipt_items ri
                      join receipts r on ri.receipt_id = r.id
                      {where}
                      """;

        try
        {
            using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            return await connection.ExecuteScalarAsync<decimal>(sql, parameters);
        }
        catch (Exception ex)
        {
            return Failure.Fatal(
                code: "DB_EXCEPTION",
                message: ex.Message,
                metadata: new Dictionary<string, object> { { "StackTrace", ex.StackTrace ?? "" } });
        }
    }

    private static (List<string> whereClauses, DynamicParameters parameters) BuildWhereClausesAndParameters(
        IEnumerable<ReceiptItemFilter> filters)
    {
        List<string> whereClauses = [];
        DynamicParameters parameters = new();

        int idx = 0;
        foreach (ReceiptItemFilter f in filters)
        {
            if (f.Stores.Any())
            {
                whereClauses.Add($"r.store = any(@stores{idx})");
                parameters.Add($"stores{idx}", f.Stores.ToArray());
            }

            if (f.Items.Any())
            {
                whereClauses.Add($"ri.item = any(@items{idx})");
                parameters.Add($"@items{idx}", f.Items.ToArray());
            }

            if (f.Categories.Any())
            {
                whereClauses.Add($"ri.category = any(@categories{idx})");
                parameters.Add($"@categories{idx}", f.Categories.ToArray());
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

            if (f.UnitPriceMin.HasValue)
            {
                whereClauses.Add($"ri.unit_price >= @upmin{idx}");
                parameters.Add($"upmin{idx}", f.UnitPriceMin.Value);
            }

            if (f.UnitPriceMax.HasValue)
            {
                whereClauses.Add($"ri.unit_price <= @upmax{idx}");
                parameters.Add($"upmax{idx}", f.UnitPriceMax.Value);
            }

            if (f.QuantityMin.HasValue)
            {
                whereClauses.Add($"ri.quantity >= @qmin{idx}");
                parameters.Add($"qmin{idx}", f.QuantityMin.Value);
            }

            if (f.QuantityMax.HasValue)
            {
                whereClauses.Add($"ri.quantity <= @qmax{idx}");
                parameters.Add($"qmax{idx}", f.QuantityMax.Value);
            }

            if (f.DiscountMin.HasValue)
            {
                whereClauses.Add($"coalesce(ri.discount, 0) >= @dmin{idx}");
                parameters.Add($"dmin{idx}", f.DiscountMin.Value);
            }

            if (f.DiscountMax.HasValue)
            {
                whereClauses.Add($"coalesce(ri.discount, 0) <= @dmax{idx}");
                parameters.Add($"dmax{idx}", f.DiscountMax.Value);
            }

            if (f.TotalMin.HasValue)
            {
                whereClauses.Add($"(ri.unit_price * ri.quantity - coalesce(ri.discount, 0)) >= @tmin{idx}");
                parameters.Add($"tmin{idx}", f.TotalMin.Value);
            }

            if (f.TotalMax.HasValue)
            {
                whereClauses.Add($"(ri.unit_price * ri.quantity - coalesce(ri.discount, 0)) <= @tmax{idx}");
                parameters.Add($"tmax{idx}", f.TotalMax.Value);
            }

            if (f.Description is not null)
            {
                whereClauses.Add($"ri.description like @desc{idx}");
                parameters.Add($"desc{idx}", $"%{f.Description}%");
            }

            idx++;
        }

        return (whereClauses, parameters);
    }

    private sealed record ReceiptItemDto(
        Guid Id,
        Guid ReceiptId,
        string Store,
        DateOnly PurchaseDate,
        string Item,
        string Category,
        decimal UnitPrice,
        decimal Quantity,
        decimal? Discount,
        string? Description);
}