using DotMaybe;

namespace ExpenseExplorer.Application.Domain.ValueObjects;

public readonly record struct ReceiptId
{
    public Guid Value { get; }

    private ReceiptId(Guid value) => Value = value;

    public static ReceiptId Unique() => new(Guid.CreateVersion7());
    public static Maybe<ReceiptId> TryCreate(Guid value) =>
        value != Guid.Empty
            ? Some.With(new ReceiptId(value))
            : None.OfType<ReceiptId>();
}