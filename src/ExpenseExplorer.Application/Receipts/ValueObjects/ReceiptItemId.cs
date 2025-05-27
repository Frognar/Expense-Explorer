namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct ReceiptItemId
{
    public Guid Value { get; }

    private ReceiptItemId(Guid value) => Value = value;

    public static ReceiptItemId Unique() => new(Guid.CreateVersion7());
}