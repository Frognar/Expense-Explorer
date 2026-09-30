using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Infrastructure.Persistence;

namespace ExpenseExplorer.Infrastructure.Receipts;

/// <summary>Pure translations between storage rows and the domain model.</summary>
internal static class ReceiptMapping
{
    /// <summary>
    /// Rows are checked with the same rules as user input. Rows that break them mean the
    /// database holds data the domain does not accept, which is reported instead of hidden.
    /// </summary>
    public static Receipt ToDomain(ReceiptRow row) =>
        ResultCombine.Combine(
                ReceiptId.Create(row.Id),
                StoreName.Create(row.Store),
                // The date was checked against "today" when it was saved; it only has to stay in the past then.
                PurchaseDate.Create(row.PurchaseDate, DateOnly.MaxValue),
                ResultCombine.Sequence(row.Items.OrderBy(i => i.Position).Select(ToDomainItem)),
                (id, store, date, items) => (id, store, date, items))
            .Bind(parts => Receipt.Restore(parts.id, parts.store, parts.date, parts.items))
            .Match(
                receipt => receipt,
                errors => throw new InvalidDataException(
                    $"Receipt {row.Id} has invalid stored data: {string.Join(", ", errors.Select(e => $"{e.Code} ({e.Target})"))}"));

    public static ReceiptRow ToNewRow(Receipt receipt)
    {
        ReceiptRow row = new() { Id = receipt.Id.Value };
        CopyTo(receipt, row);
        return row;
    }

    /// <summary>Makes <paramref name="row"/> mirror <paramref name="receipt"/>: header, changed, added and removed lines.</summary>
    public static void CopyTo(Receipt receipt, ReceiptRow row)
    {
        row.Store = receipt.Store.Value;
        row.PurchaseDate = receipt.PurchaseDate.Value;

        HashSet<Guid> currentIds = [.. receipt.Items.Select(item => item.Id.Value)];
        row.Items.RemoveAll(itemRow => !currentIds.Contains(itemRow.Id));

        Dictionary<Guid, ReceiptItemRow> rowsById = row.Items.ToDictionary(itemRow => itemRow.Id);
        for (int position = 0; position < receipt.Items.Count; position++)
        {
            ReceiptItem item = receipt.Items[position];
            if (!rowsById.TryGetValue(item.Id.Value, out ReceiptItemRow? itemRow))
            {
                itemRow = new ReceiptItemRow { Id = item.Id.Value, ReceiptId = row.Id };
                row.Items.Add(itemRow);
            }

            CopyTo(item.Purchase, position, itemRow);
        }
    }

    private static void CopyTo(Purchase purchase, int position, ReceiptItemRow row)
    {
        row.Position = position;
        row.Item = purchase.Item.Value;
        row.Category = purchase.Category.Value;
        row.Quantity = purchase.Price.Quantity.Value;
        row.UnitPrice = purchase.Price.UnitPrice.Value;
        row.Discount = purchase.Price.Discount.Value;
        row.Description = purchase.Description?.Value;
    }

    private static Result<(ReceiptItemId, Purchase)> ToDomainItem(ReceiptItemRow row) =>
        ResultCombine.Combine(
                ReceiptItemId.Create(row.Id).ForTarget($"items[{row.Id}].id"),
                ItemName.Create(row.Item).ForTarget($"items[{row.Id}].item"),
                CategoryName.Create(row.Category).ForTarget($"items[{row.Id}].category"),
                ToDomainPrice(row).ForTarget($"items[{row.Id}].price"),
                Description.CreateOptional(row.Description).ForTarget($"items[{row.Id}].description"),
                (id, item, category, price, description) => (id, new Purchase(item, category, price, description)));

    private static Result<LinePrice> ToDomainPrice(ReceiptItemRow row) =>
        ResultCombine.Combine(
                Quantity.Create(row.Quantity),
                UnitPrice.Create(row.UnitPrice),
                Money.Create(row.Discount),
                (quantity, unitPrice, discount) => (quantity, unitPrice, discount))
            .Bind(parts => LinePrice.Create(parts.quantity, parts.unitPrice, parts.discount));
}
