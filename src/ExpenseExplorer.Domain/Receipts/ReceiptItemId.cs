using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Domain.Receipts;

public sealed record ReceiptItemId
{
    private ReceiptItemId(Guid value) => Value = value;

    public Guid Value { get; }

    public static ReceiptItemId New() => new(Guid.CreateVersion7());

    public static Result<ReceiptItemId> Create(Guid value) =>
        value == Guid.Empty
            ? Result.Failure<ReceiptItemId>(new Error("ReceiptItemId.Empty", "Identifier cannot be empty."))
            : Result.Success(new ReceiptItemId(value));

    public override string ToString() => Value.ToString();
}
