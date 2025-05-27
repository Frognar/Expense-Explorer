namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct ReceiptId
{
    public Guid Value { get; }

    private ReceiptId(Guid value) => Value = value;

    public static ReceiptId Unique() => new(Guid.CreateVersion7());
    public static Option<ReceiptId> TryCreate(string value) =>
        Guid.TryParse(value, out Guid guid)
            ? Option.Some(new ReceiptId(guid))
            : Option.None<ReceiptId>();
}