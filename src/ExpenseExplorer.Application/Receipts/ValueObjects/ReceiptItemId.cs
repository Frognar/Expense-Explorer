namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct ReceiptItemId
{
    public Guid Value { get; }

    private ReceiptItemId(Guid value) => Value = value;

    public static ReceiptItemId Unique() => new(Guid.CreateVersion7());
    public static Option<ReceiptItemId> TryCreate(Guid value) =>
        value != Guid.Empty
            ? Option.Some(new ReceiptItemId(value))
            : Option.None<ReceiptItemId>();
}