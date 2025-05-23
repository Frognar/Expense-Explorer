namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct ReceiptId
{
    public Guid Value { get; }

    private ReceiptId(Guid value) => Value = value;

    public static ReceiptId Unique() => new(Guid.CreateVersion7());
}