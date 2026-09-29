using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record ReceiptId
{
    private ReceiptId(Guid value) => Value = value;

    public Guid Value { get; }

    public static ReceiptId New() => new(Guid.CreateVersion7());

    public static Result<ReceiptId> Create(Guid value) =>
        value == Guid.Empty
            ? Result.Failure<ReceiptId>(new Error("ReceiptId.Empty", "Identifier cannot be empty."))
            : Result.Success(new ReceiptId(value));

    public override string ToString() => Value.ToString();
}
