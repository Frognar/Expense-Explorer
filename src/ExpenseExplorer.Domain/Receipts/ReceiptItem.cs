namespace ExpenseExplorer.Domain.Receipts;

/// <summary>A line of a <see cref="Receipt"/>. It can only be changed through its receipt.</summary>
public sealed class ReceiptItem
{
    internal ReceiptItem(ReceiptItemId id, Purchase purchase)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(purchase);

        Id = id;
        Purchase = purchase;
    }

    public ReceiptItemId Id { get; }

    public Purchase Purchase { get; private set; }

    public Money Total => Purchase.Price.Total;

    internal void Replace(Purchase purchase)
    {
        ArgumentNullException.ThrowIfNull(purchase);
        Purchase = purchase;
    }
}
